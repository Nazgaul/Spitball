using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.MobileApi.Extensions;
using Cloudents.Web.Extensions.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.MobileApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// The controller of job api
    /// </summary>
    [Route("api/[controller]", Name = "Job")]
    public class JobController : Controller
    {
        private readonly IJobSearch _jobSearch;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jobSearch">job search provider</param>
        public JobController(IJobSearch jobSearch)
        {
            _jobSearch = jobSearch;
        }

        /// <summary>
        /// Query to get jobs vertical
        /// </summary>
        /// <param name="model">The model to transfer</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery]JobRequest model, CancellationToken token)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await _jobSearch.SearchAsync(model.Term,
                model.Sort.GetValueOrDefault(JobRequestSort.Relevance),
                model.Facet, model.Location, model.Page.GetValueOrDefault(), model.Highlight, token).ConfigureAwait(false);
            string nextPageLink = null;
            if (result.Result?.Any() == true)
            {
                nextPageLink = Url.NextPageLink("Job", model);
            }

            return Ok(
                new
                {
                    result,
                    nextPageLink
                });
        }
    }
}