﻿using System;
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
    /// <summary>
    /// The controller of job api
    /// </summary>
    [MobileAppController]
    public class JobController : ApiController
    {
        private readonly IJobSearch _jobSearch;

        public JobController(IJobSearch jobSearch)
        {
            _jobSearch = jobSearch;
        }

        //[TypeFilter(typeof(IpToLocationActionFilter), Arguments = new object[] { "location" })]
        /// <summary>
        /// Query to get jobs vertical
        /// </summary>
        /// <param name="model">The model to transfer</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Get([FromUri]JobRequest model, CancellationToken token)
        {
            if (model.Term == null) throw new ArgumentNullException(nameof(model.Term));
            var result = await _jobSearch.SearchAsync(string.Join(" ", model.Term),
                model.Filter.GetValueOrDefault(), model.Sort.GetValueOrDefault(JobRequestSort.Distance),
                model.Facet, model.Location, model.Page.GetValueOrDefault(), token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }

        /// <summary>
        /// Get Job with next page token add to query string api-version = 2017-12-19
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [VersionedRoute("api/job", "2017-12-19", Name = "JobSearch"), HttpGet]
        public async Task<HttpResponseMessage> JobV2Async([FromUri]JobRequest model, CancellationToken token)
        {
            if (model.Term == null) throw new ArgumentNullException(nameof(model.Term));
            var result = await _jobSearch.SearchAsync(string.Join(" ", model.Term),
                model.Filter.GetValueOrDefault(), model.Sort.GetValueOrDefault(JobRequestSort.Distance),
                model.Facet, model.Location, model.Page.GetValueOrDefault(), token).ConfigureAwait(false);

            var nextPageLink = Url.NextPageLink("JobSearch", new
            {
                api_version = "2017-12-19"
            }, model);

            return Request.CreateResponse(
                new
                {
                    result,
                    nextPageLink
                });
        }
    }
}