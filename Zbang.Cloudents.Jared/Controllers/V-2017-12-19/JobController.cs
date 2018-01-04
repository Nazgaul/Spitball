using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Extensions;
using Zbang.Cloudents.Jared.Filters;
using Zbang.Cloudents.Jared.Models;

namespace Zbang.Cloudents.Jared.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// The controller of job api
    /// </summary>
    [MobileAppController ]
    public class Job2Controller : ApiController
    {
        private readonly IJobSearch _jobSearch;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="jobSearch">job search provider</param>
        public Job2Controller(IJobSearch jobSearch)
        {
            _jobSearch = jobSearch;
        }

        /// <summary>
        /// Get Job with next page token add to query string api-version = 2017-12-19
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [VersionedRoute("api/job", "2017-12-19", Name = "JobSearch"), HttpGet]

        public async Task<IHttpActionResult> JobV2Async([FromUri]JobRequest model, CancellationToken token)
        {
            var result = await _jobSearch.SearchAsync(model.Term,
                model.Filter.GetValueOrDefault(), model.Sort.GetValueOrDefault(JobRequestSort.Distance),
                model.Facet, model.Location, model.Page.GetValueOrDefault(), token).ConfigureAwait(false);

            var nextPageLink = Url.NextPageLink("JobSearch", new
            {
                api_version = "2017-12-19"
            }, model);

            return Ok(
                new
                {
                    result,
                    nextPageLink
                });
        }
    }
}