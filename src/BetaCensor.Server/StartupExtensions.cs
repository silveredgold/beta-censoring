using BetaCensor.Core.Messaging;
using CensorCore;
using CensorCore.Censoring;
using CensorCore.ModelLoader;
using MediatR;

namespace BetaCensor.Server;

public static class StartupExtensions {

    public static IServiceCollection AddBehaviors(this IServiceCollection services) {
        return services.AddSingleton<IPipelineBehavior<CensorImageRequest, CensorImageResponse>, BetaCensor.Server.Messaging.ConnectionResponseBehaviour>();
    }

    public static IServiceCollection AddCensoring(this IServiceCollection services, byte[] model) {

        services.AddSingleton<IImageHandler>(p => new ImageSharpHandler());
        services.AddSingleton<ModelLoader>(p =>
        {
            return new ModelLoaderBuilder()
                .AddDefaultPaths()
                // .AddSearchPath(config)
                .SearchAssembly(System.Reflection.Assembly.GetEntryAssembly())
                .Build();
        });
        services.AddSingleton<AIService>(p =>
        {
            // var loader = p.GetRequiredService<ModelLoader>();
            // var model = await loader.GetModel();
            if (model == null) {
                throw new InvalidOperationException("Could not load model from any available source!");
            }
            return AIService.Create(model, p.GetRequiredService<IImageHandler>(), false);
        });

        services.AddSingleton<IAssetStore, EmptyAssetStore>();
        services.AddSingleton<GlobalCensorOptions>();
        services.AddSingleton<ICensorTypeProvider, BlurProvider>();
        services.AddSingleton<ICensorTypeProvider, PixelationProvider>();
        services.AddSingleton<ICensorTypeProvider, BlackBarProvider>();
        services.AddSingleton<ICensorTypeProvider, StickerProvider>();
        services.AddSingleton<ICensoringProvider, ImageSharpCensoringProvider>();
        return services;
    }

    public static MatchOptions BuildMatchOptions(this IServiceProvider provider) {
        Console.WriteLine("Preparing match options factory!");
        var config = provider.GetRequiredService<IConfiguration>();
        var logger = provider.GetRequiredService<ILogger<MatchOptions>>();
        logger.LogInformation("Getting match options from configuration!");
        var section = config.GetSection("MatchOptions");
        var defaults = MatchOptions.GetDefault();
        if (section.Exists() && section.Get<MatchOptions>() is var matchOpts && matchOpts is not null) {
            logger.LogInformation("Valid configuration section found, parsing for match options!");
            defaults.MinimumScore = matchOpts.MinimumScore == default(float) ? defaults.MinimumScore : matchOpts.MinimumScore;
            matchOpts.ClassScores ??= new();
            foreach (var classScore in matchOpts.ClassScores)
            {
                defaults.ClassScores[classScore.Key] = classScore.Value;
            }
        }
        return defaults;
    }
}