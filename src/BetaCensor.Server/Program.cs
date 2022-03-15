using BetaCensor.Core.Messaging;
using BetaCensor.Server;
using BetaCensor.Server.Discovery;
using BetaCensor.Server.Messaging;
using BetaCensor.Web;
using BetaCensor.Workers;
using CensorCore;
using CensorCore.ModelLoader;
using ConfigurEngine;
using MediatR;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.FileProviders;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:2382");
// builder.WebHost.AdvertiseServer();

builder.Configuration
    .AddConfigFile("config")
    .AddConfigFile("stickers")
    .AddConfigFile("beta-config");

builder.Host.UseSystemd();
builder.Host.UseWindowsService();

// Add services to the container.

var loader = new ModelLoaderBuilder()
        .AddDefaultPaths()
        // .AddSearchPath(config)
        .SearchAssembly(System.Reflection.Assembly.GetEntryAssembly())
        .Build();
var model = await loader.GetModel();
if (model == null) throw new InvalidDataException("Failed to retrieve AI model! Aborting...");
builder.Services.AddCensoring(model);

var serverOpts = builder.Configuration.GetSection("Server").Get<ServerOptions>();
serverOpts ??= new ServerOptions();

// builder.Services.AddSpaStaticFiles(options => {options.RootPath = "wwwroot";});
var mvc = builder.Services.AddControllers();
if (serverOpts.EnableRest) {
    Console.WriteLine("Enabling REST interface!");
    mvc.AddApplicationPart(typeof(CensorCore.Web.CensoringController).Assembly);
}
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRazorPages();

var physicalProvider = builder.Environment.ContentRootFileProvider;
var manifestEmbeddedProvider =
    new ManifestEmbeddedFileProvider(typeof(Program).Assembly, "wwwroot");
var compositeProvider =
    new CompositeFileProvider(physicalProvider, manifestEmbeddedProvider);
builder.Services.AddSingleton<IFileProvider>(compositeProvider);

builder.Services.AddMediatR(conf =>
{
}, typeof(Program), typeof(AIService), typeof(BetaCensor.Core.Messaging.CensorImageRequest));

builder.Services.AddBehaviors();

if (serverOpts.EnableSignalR) {
    Console.WriteLine("Enabling SignalR interface!");
    builder.Services.AddSignalR(o =>
    {
        o.EnableDetailedErrors = true;
    }).AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
        options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.PayloadSerializerOptions.WriteIndented = true;
    }).AddHubOptions<BetaCensor.Server.Controllers.CensoringHub>(o =>
    {
        o.MaximumReceiveMessageSize = 4194304;
    });
}

if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !string.IsNullOrWhiteSpace(serverOpts.SocketPath)) {
    builder.WebHost.ConfigureKestrel(k =>
    {
        k.ListenUnixSocket(serverOpts.SocketPath);
    });
}

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.AddQueues<CensorImageRequest, CensorImageResponse>();
builder.Services.AddSingleton<QueueValidator<CensorImageRequest>>(p => new QueueValidator<CensorImageRequest>(req => req.RequestId, p.GetRequiredService<ILogger<QueueValidator<CensorImageRequest>>>()));
builder.Services.AddHostedService<NotificationWorkerService>();
builder.Services.AddWorkers<DispatchWorkerService<CensorImageRequest, CensorImageResponse>>(builder.Configuration.GetSection("Server"));

builder.Services.AddHostedService<DiscoveryService>();

var stickerOpts = builder.Configuration.GetSection("Stickers");
builder.Services.AddStickerService(stickerOpts, builder.Environment);

builder.Services.AddScoped<MatchOptions>(ServerConfigurationExtensions.BuildMatchOptions);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(e =>
{
    e.MapHub<BetaCensor.Server.Controllers.CensoringHub>("/live", conf => conf.ApplicationMaxBufferSize = 4194304);
});
// app.MapHub<BetaCensor.Server.Controllers.CensoringHub>("/live");

app.MapControllers();
app.UseWebSockets();
if (app.Environment.IsDevelopment()) {
    app.UseSpa(spa =>
    {
        spa.Options.SourcePath = "ClientApp";
        spa.Options.DevServerPort = 3000;
        spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
    });
}
else {
    app.UseStaticFiles(new StaticFileOptions {
        FileProvider = compositeProvider
    });
}
app.MapRazorPages();
app.Run();
