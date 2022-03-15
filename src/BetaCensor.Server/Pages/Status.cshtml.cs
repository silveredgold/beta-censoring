using System.Diagnostics;
using System.Reflection;
using BetaCensor.Core.Messaging;
using BetaCensor.Workers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BetaCensor.Server.Pages;

public class StatusModel : PageModel {
    private readonly ILogger<StatusModel> logger;

    public StatusModel(ILogger<StatusModel> logger) {
        this.logger = logger;
    }
    public IActionResult OnGetRequestCount([FromServices] IAsyncBackgroundQueue<CensorImageRequest> requestQueue) {
        var count = requestQueue.GetItemCount() ?? -1;
        return new JsonResult(new { requests = count });
    }

    public IActionResult OnGetServerVersion() {
        var assembly = typeof(Controllers.CensoringHub).Assembly;
        var version = GetProductVersion(assembly);
        if (!string.IsNullOrWhiteSpace(version)) {
            return new JsonResult(new { version = $"v{version}", hostname = Environment.MachineName });
        }
        else {
            return new JsonResult(new { version = "Unknown", hostname = Environment.MachineName });
        }
    }

    public IActionResult OnGetServerConfiguration([FromServices] IConfiguration config) {
        var options = config.GetSection("Server").Get<ServerOptions>() ?? new ServerOptions();
        return new JsonResult(new
        {
            services = new
            {
                rest = options.EnableRest,
                signalr = options.EnableSignalR
            },
            workers = options.WorkerCount,
            hostname = Environment.MachineName
        });
    }

    private static string? GetProductVersion(Assembly assembly) {
        try {
            object[] attributes = assembly
            .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            return attributes.Length == 0 ?
                null :
                ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion;
        }
        catch {
            return null;
        }
    }

    public async Task<IActionResult> OnGetAssets([FromServices] Web.StickerProvider stickerProvider) {
        var stickers = await stickerProvider.GetCategories();
        return new JsonResult(new
        {
            stickerCategories = stickers
        });
    }
}
