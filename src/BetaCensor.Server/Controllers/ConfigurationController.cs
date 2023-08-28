using BetaCensor.Caching;
using BetaCensor.Web;
using CensorCore.Censoring;
using Microsoft.AspNetCore.Mvc;

namespace BetaCensor.Server.Controllers
{
    [ApiController]
    [Route("_server")]
    public class ServerConfigController : ControllerBase
    {
        [PreferYamlFilter]
        [HttpGet("options")]
        public IActionResult GetMatchOptions(
            [FromServices] CensorCore.MatchOptions matchOpts, 
            [FromServices] GlobalCensorOptions censorOptions, 
            [FromServices] CachingOptions cachingOptions, 
            [FromServices] IConfiguration config) {
            var serverConf = config.GetServerOptions();
            var stickerSection = config.GetSection("Stickers");
                var captionSection = config.GetSection("Captions");
                var opts = stickerSection.Get<StickerOptions>() ?? new StickerOptions();
                var captionOpts = captionSection.Get<CaptionOptions>() ?? new CaptionOptions();
            return Ok(new {
                Server = serverConf,
                MatchOptions = matchOpts,
                Captions = captionOpts,
                Stickers = opts,
                Caching = cachingOptions,
                CensorOptions = censorOptions
            });
        }
    }
}