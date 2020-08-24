using System;
using Cloudents.Core;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Query;
using Cloudents.Query.HomePage;
using Cloudents.Web.Binders;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class HomePageController : ControllerBase
    {

        private readonly ConfigurationService _versionService;
        private readonly IQueryBus _queryBus;
        private readonly IUrlBuilder _urlBuilder;

        public HomePageController(ConfigurationService versionService, IQueryBus queryBus, IUrlBuilder urlBuilder)
        {
            _versionService = versionService;
            _queryBus = queryBus;
            _urlBuilder = urlBuilder;
        }

        [HttpGet("version")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Version()
        {
            return Ok(new { version = _versionService.GetVersion() });
        }


        [HttpGet("banner"),Obsolete]
        public object GetTopBannerAsync()
        {
            //var query = new GetBannerQuery(CultureInfo.CurrentCulture);
            // var retValTask = await _queryBus.QueryAsync(query, token);
            // if (retValTask == null)
            // {
            Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = TimeSpan.FromDays(1),
                MaxStale = true,
                MaxStaleLimit = TimeSpan.FromDays(1),


            };
            Response.Headers.Remove("Pragma");
            return null;
            //Response.GetTypedHeaders(). = new EntityTagHeaderValue(new StringSegment("\"a\""),false);
            //}
            // return retValTask;
        }

        /// <summary>
        /// Get tutor reviews for home page
        /// </summary>
        /// <param name="count">The amount of tutors</param>
        /// <param name="profile">Ignore</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("reviews")]
        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = TimeConst.Week, VaryByQueryKeys = new[] { "count" })]

        public async Task<IEnumerable<ReviewDto>> GetReviewsAsync(int count,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            var query = new ReviewsQuery(profile.CountryRegion, count);
            var result = await _queryBus.QueryAsync(query, token);

            return result.Select(s =>
            {
                s.TutorImage = _urlBuilder.BuildUserImageEndpoint(s.TutorId, s.TutorImage);
                return s;
            });

        }

        [HttpGet]
        [ResponseCache(Duration = TimeConst.Day, Location = ResponseCacheLocation.Any)]
        public async Task<StatsDto> GetStatsAsync(CancellationToken token)
        {
            var query = new StatsQuery();
            var retValTask = await _queryBus.QueryAsync(query, token);
            return retValTask;
        }



    }
}
