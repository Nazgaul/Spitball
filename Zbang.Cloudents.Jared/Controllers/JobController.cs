using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Microsoft.Azure.Mobile.Server.Config;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class JobController : ApiController
    {
        private readonly IJobSearch _jobSearch;

        public JobController(IJobSearch jobSearch)
        {
            _jobSearch = jobSearch;
        }

        //[TypeFilter(typeof(IpToLocationActionFilter), Arguments = new object[] { "location" })]
        public async Task<HttpResponseMessage> Get(string[] term,
            JobRequestFilter? filter,
            JobRequestSort? sort,
            GeoPoint location, string[] facet, CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));
            var result = await _jobSearch.SearchAsync(string.Join(" ", term), filter.GetValueOrDefault(), sort.GetValueOrDefault(JobRequestSort.Distance),
                facet, location, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}