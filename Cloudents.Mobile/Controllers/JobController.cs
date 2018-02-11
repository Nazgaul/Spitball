using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Mobile.Extensions;
using Cloudents.Mobile.Models;
using Microsoft.Azure.Mobile.Server.Config;

namespace Cloudents.Mobile.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// The controller of job api
    /// </summary>
    [MobileAppController]
    public class JobController : ApiController
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
        public async Task<IHttpActionResult> Get([FromUri]JobRequest model, CancellationToken token)
        {
            var facet = model.Facet?.Select(s =>
            {
                if (s.TryToEnum(out JobFilter p))
                {
                    return p;
                }

                return JobFilter.None;
            }).Where(w=> w != JobFilter.None);
            var result = await _jobSearch.SearchAsync(model.Term,
                model.Sort.GetValueOrDefault(JobRequestSort.Distance),
                facet, model.Location, model.Page.GetValueOrDefault(), model.Highlight, token).ConfigureAwait(false);
            string nextPageLink = null;
            if (result.Result?.Any() == true)
            {
                nextPageLink = Url.NextPageLink("DefaultApis", null, model);
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