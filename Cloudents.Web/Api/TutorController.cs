﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Tutor api controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]", Name = "Tutor"), ApiController]
    public class TutorController : ControllerBase
    {
        private readonly ITutorSearch _tutorSearch;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tutorSearch"></param>
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
        [HttpGet]
        public async Task<WebResponseWithFacet<TutorDto>> GetAsync([FromQuery]TutorRequest model, CancellationToken token)
        {
            var t = Request.GetCapabilities();
            var isMobile = t?.IsMobileDevice ?? true;
            var result = (await _tutorSearch.SearchAsync(model.Term,
                model.Filter,
                model.Sort.GetValueOrDefault(TutorRequestSort.Relevance),
                model.Location?.ToGeoPoint(),
                model.Page.GetValueOrDefault(), isMobile, token).ConfigureAwait(false)).ToListIgnoreNull();
            string nextPageLink = null;
            if (result.Count > 0)
            {
                nextPageLink = Url.NextPageLink("Tutor", null, model);
            }

            return new WebResponseWithFacet<TutorDto>
            {
                Result = result,
                Sort = Enum.GetNames(typeof(TutorRequestSort)),
                Filters = new[]
                {
                    new Models.Filters(nameof(TutorRequest.Filter),"Status", Enum.GetNames(typeof(TutorRequestFilter)))
                },
                NextPageLink = nextPageLink
            };
        }
    }
}
