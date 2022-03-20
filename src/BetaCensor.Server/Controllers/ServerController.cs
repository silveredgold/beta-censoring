using BetaCensor.Server.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace BetaCensor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServerController : ControllerBase
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