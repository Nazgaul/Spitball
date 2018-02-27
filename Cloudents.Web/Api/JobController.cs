using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions.Models;
using JetBrains.Annotations;
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

        [HttpGet]
        public async Task<IActionResult> GetAsync([NotNull] [FromQuery]JobRequest model,
             CancellationToken token)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            var result = await _jobSearch.SearchAsync(
                model.Term,
                model.Sort.GetValueOrDefault(JobRequestSort.Distance),
                model.Facet, model.Location, model.Page.GetValueOrDefault(), true, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}