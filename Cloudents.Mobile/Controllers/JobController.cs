using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Enum;
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
        private readonly IIpToLocation _ipToLocation;

        /// <inheritdoc />
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="jobSearch">job search provider</param>
        /// <param name="ipToLocation"></param>
        public JobController(IJobSearch jobSearch, IIpToLocation ipToLocation)
        {
            _jobSearch = jobSearch;
            _ipToLocation = ipToLocation;
        }

        /// <summary>
        /// Query to get jobs vertical
        /// </summary>
        /// <param name="model">The model to transfer</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Get([FromUri]JobRequest model, CancellationToken token)
        {
            var location = Request.GetClientIp();
            var locationObj = await _ipToLocation.GetAsync(IPAddress.Parse(location), token).ConfigureAwait(false);
            if (model.Location != null)
            {
                locationObj.Point = model.Location;
            }
            var result = await _jobSearch.SearchAsync(model.Term,
                model.Filter.GetValueOrDefault(), model.Sort.GetValueOrDefault(JobRequestSort.Distance),
                model.Facet, locationObj, model.Page.GetValueOrDefault(), token).ConfigureAwait(false);

            var nextPageLink = Url.NextPageLink("DefaultApis", null, model);
            return Ok(
                new
                {
                    result,
                    nextPageLink
                });
        }


    }
}