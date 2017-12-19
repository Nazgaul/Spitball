﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Extensions;
using Zbang.Cloudents.Jared.Filters;
using Zbang.Cloudents.Jared.Models;

namespace Zbang.Cloudents.Jared.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Tutor api controller
    /// </summary>
    [MobileAppController]
    public class TutorController : ApiController
    {
        private readonly ITutorSearch _tutorSearch;

        public TutorController(ITutorSearch tutorSearch)
        {
            _tutorSearch = tutorSearch;
        }

        /// <summary>
        /// Get Tutors
        /// </summary>
        /// <param name="model">The model to parse</param>
        /// <param name="token"></param>
        /// <returns></returns>
        //TODO we need geo point to be on in here since case 8920
        public async Task<HttpResponseMessage> Get([FromUri]TutorRequest model, CancellationToken token)
        {
            var result = await _tutorSearch.SearchAsync(string.Join(" ", model.Term),
                model.Filter,
                model.Sort.GetValueOrDefault(TutorRequestSort.Price),
                model.Location,
                model.Page.GetValueOrDefault(), token).ConfigureAwait(false);

            return Request.CreateResponse(
                result
                );
        }

        /// <summary>
        /// Get Tutors with next page token add to query string api-version = 2017-12-19
        /// </summary>
        /// <param name="model">The model to parse</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [VersionedRoute("api/tutor", "2017-12-19", Name = "TutorSearch"), HttpGet]
        public async Task<HttpResponseMessage> TutorV2Async([FromUri]TutorRequest model, CancellationToken token)
        {
            var result = await _tutorSearch.SearchAsync(string.Join(" ", model.Term),
                model.Filter,
                model.Sort.GetValueOrDefault(TutorRequestSort.Price),
                model.Location,
                model.Page.GetValueOrDefault(), token).ConfigureAwait(false);
            var nextPageLink = Url.NextPageLink("TutorSearch", new
            {
                api_version = "2017-12-19"
            }, model);
            return Request.CreateResponse(new
            {
                result,
                nextPageLink
            });
        }
    }
}
