using CensorCore.Censoring;
using Microsoft.AspNetCore.Mvc;

namespace BetaCensor.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssetsController : ControllerBase
    {
        [HttpGet("categories")]
        public IActionResult GetCategories([FromServices]IStickerProvider provider, [FromQuery]string type) {
            if (string.Equals(type, KnownAssetTypes.Stickers, StringComparison.CurrentCultureIgnoreCase)) {
                return Ok(provider.GetCategories());
            } else {
                return Ok(Array.Empty<string>());
            }
        }
    }
}