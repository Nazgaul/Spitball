using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ReportController : Controller
    {

        private readonly IServiceBusProvider _serviceBus;

        public ReportController(IServiceBusProvider serviceBus)
        {
            _serviceBus = serviceBus;
        }

        // GET
        [HttpPost("csp")]
        public async Task<IActionResult> PostAsync(string report, CancellationToken token)
        {
            // await _serviceBus.InsertMessageAsync(new ReportEmail("csp report", report.ToString()), token);
            return Ok();
        }
    }
}