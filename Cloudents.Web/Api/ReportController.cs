using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true), ApiController]
    public class ReportController : ControllerBase
    {

        //private readonly IServiceBusProvider _serviceBus;

        //public ReportController(IServiceBusProvider serviceBus)
        //{
        //    _serviceBus = serviceBus;
        //}

        // GET
        [HttpPost("csp")]
        public IActionResult PostAsync(string report, CancellationToken token)
        {
            // await _serviceBus.InsertMessageAsync(new ReportEmail("csp report", report.ToString()), token);
            return Ok();
        }
    }
}