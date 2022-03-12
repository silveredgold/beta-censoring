using BetaCensor.Web;
using CensorCore.Censoring;
using Microsoft.AspNetCore.Mvc;
using StickerProvider = BetaCensor.Web.StickerProvider;

namespace BetaCensor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssetsController : ControllerBase
    {
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories([FromServices]StickerProvider provider, [FromQuery]string type) {
            if (string.Equals(type, KnownAssetTypes.Stickers, StringComparison.CurrentCultureIgnoreCase)) {
                return Ok(await provider.GetCategories());
            } else {
                return Ok(Array.Empty<string>());
            }
        }
    }
}