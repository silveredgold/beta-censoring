using BetaCensor.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BetaCensor.Web.Status.Pages;

public class StatusModel : PageModel {
    private readonly ILogger<StatusModel> logger;

    public StatusModel(ILogger<StatusModel> logger) {
        this.logger = logger;
    }
    public IActionResult OnGetRequestCount([FromServices] IServerInfoService infoService) {
        var count = infoService.GetRequestCount();
        return new JsonResult(new { requests = count });
    }

    public IActionResult OnGetServerVersion([FromServices] Web.Status.IVersionService versionService) {
        var version = versionService.GetServerVersion();
        if (!string.IsNullOrWhiteSpace(version)) {
            return new JsonResult(new { version = $"v{version}", hostname = Environment.MachineName });
        }
        else {
            return new JsonResult(new { version = "Unknown", hostname = Environment.MachineName });
        }
    }

    public IActionResult OnGetServerConfiguration([FromServices] IServerInfoService infoService) {
        var enabledServices = infoService.GetServices() ?? new Dictionary<string, bool>();
        var components = infoService.GetComponents();
        return new JsonResult(new
        {
            services = enabledServices,
            hostname = Environment.MachineName,
            components = components
        });
    }

    

    public async Task<IActionResult> OnGetAssets([FromServices] Web.StickerProvider stickerProvider) {
        var stickers = await stickerProvider.GetCategories();
        return new JsonResult(new
        {
            stickerCategories = stickers
        });
    }
}
