using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// The controller of job api
    /// </summary>
    [Route("api/[controller]", Name = "Job"), ApiController]
    public class JobController : ControllerBase
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

        public async Task<WebResponseWithFacet<JobDto>> GetAsync([FromQuery]JobRequest model, CancellationToken token)
        {
            var result = await _jobSearch.SearchAsync(model.Term,
                model.Sort.GetValueOrDefault(JobRequestSort.Relevance),
                model.Facet, model.Location?.ToLocation(), model.Page.GetValueOrDefault(), token).ConfigureAwait(false);
            string nextPageLink = null;
            result.Result = result.Result?.ToList();
            if (result.Result?.Any() == true)
            {
                nextPageLink = Url.NextPageLink("Job", model);
            }
            return new WebResponseWithFacet<JobDto>()
            {
                Result = result.Result,
                Filters = new Dictionary<string, IEnumerable<string>>
                {
                    ["Subject"] = result.Facet
                },
                Sort = Enum.GetNames(typeof(JobRequestSort)),
                NextPageLink = nextPageLink
            };
        }
    }
}