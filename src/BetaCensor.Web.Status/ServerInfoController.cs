
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BetaCensor.Web.Status;

[ApiController]
[Route("_server")]
public class InfoController : ControllerBase {
    [HttpGet("info")]
    public IActionResult GetCensoringPerformanceCache([FromServices] IServerInfoService infoService, [FromServices] IVersionService versionService) {
        try {
            var enabledServices = infoService.GetServices() ?? new Dictionary<string, bool>();
            var components = infoService.GetComponents() ?? new Dictionary<string, string>();
            var requests = infoService.GetRequestCount() ?? -1;
            var version = versionService.GetServerVersion() ?? "v0.0.0-unknown";
            return new JsonResult(new
            {
                services = enabledServices,
                components = components,
                requests = requests,
                version = version
            });
        }
        catch {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}