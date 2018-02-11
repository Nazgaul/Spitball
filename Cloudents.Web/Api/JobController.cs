using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
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

        public async Task<IActionResult> Get(string[] term,
            JobRequestSort? sort,
            Location location, string[] facet, int? page, CancellationToken token)
        {
            var facetEnum = facet.Select(s =>
            {
                if (s.TryToEnum(out JobFilter p))
                {
                    return p;
                }
                return JobFilter.None;
            }).Where(w => w != JobFilter.None);
            var result = await _jobSearch.SearchAsync(
                term,
                sort.GetValueOrDefault(JobRequestSort.Distance),
                facetEnum, location, page.GetValueOrDefault(), true, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}