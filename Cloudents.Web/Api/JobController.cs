using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Job")]
    public class JobController : Controller
    {
        private readonly IJobSearch _jobSearch;

        public JobController(IJobSearch jobSearch)
        {
            _jobSearch = jobSearch;
        }

        //[TypeFilter(typeof(IpToLocationActionFilter), Arguments = new object[] { "location" })]
        public async Task<IActionResult> Get(string[] term,
            JobRequestFilter? filter,
            JobRequestSort? sort,
            Location location, string[] facet,int? page, CancellationToken token)
        {
            var result = await _jobSearch.SearchAsync(
                term,
                filter.GetValueOrDefault(),
                sort.GetValueOrDefault(JobRequestSort.Distance),
                facet, location, page.GetValueOrDefault(), token).ConfigureAwait(false);
            return Json(result);
        }
    }
}