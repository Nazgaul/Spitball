using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Job")]
    public class JobController : Controller
    {

        private readonly IJobSearch m_JobSearchSearch;

        public JobController(IJobSearch jobSearch)
        {
            m_JobSearchSearch = jobSearch;
        }

        //TODO: location is not null
        public async Task<IActionResult> Get(string term,
            SearchRequestFilter filter,
            SearchRequestSort sort,
            GeoPoint location, string facet, CancellationToken token)
        {
            var result = await m_JobSearchSearch.SearchAsync(term, filter, sort, facet, location ,token).ConfigureAwait(false);
            return Json(result);
        }
    }
}