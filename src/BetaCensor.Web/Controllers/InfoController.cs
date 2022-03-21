using BetaCensor.Web.Performance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BetaCensor.Web.Controllers
{
    [ApiController]
    [Route("_server")]
    public class InfoController : ControllerBase
    {
        [HttpGet("performance")]
        public async Task<IActionResult> GetCensoringPerformanceCache([FromServices]IPerformanceDataService db) {
            try {
                // using var db = new PerformanceDataService();
                var values = (await db.GetAllRecords()).ToList();
                // HttpContext?.Response.RegisterForDispose(db);
                return Ok(values);
            } catch {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}