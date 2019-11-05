using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Models;
using Cloudents.Query.HomePage;
using Cloudents.Web.Binders;
using Cloudents.Web.Models;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class HomePageController : ControllerBase
    {

        private readonly ConfigurationService _versionService;
        private readonly IQueryBus _queryBus;

        public HomePageController(ConfigurationService versionService, IQueryBus queryBus)
        {
            _versionService = versionService;
            _queryBus = queryBus;
        }

        [HttpGet("version")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Version()
        {
            return Ok(new { version = _versionService.GetVersion() });
        }



        /// <summary>
        /// Get tutor for home page
        /// </summary>
        /// <param name="count">The amount of tutors</param>
        /// <param name="profile">Ignore</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("tutors")]
        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = TimeConst.Day, VaryByQueryKeys = new[] { "count" })]
        public async Task<IEnumerable<TutorCardDto>> GetTopTutorsAsync(int count,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            var query = new TopTutorsQuery(profile.Country, count);
            var retValTask = await _queryBus.QueryAsync(query, token);
            return retValTask;
        }


        /// <summary>
        /// Get tutor reviews for home page
        /// </summary>
        /// <param name="count">The amount of tutors</param>
        /// <param name="profile">Ignore</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("reviews")]
        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = TimeConst.Day, VaryByQueryKeys = new []{"count"})]

        public async Task<IEnumerable<ReviewDto>> GetReviewsAsync(int count,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            var query = new ReviewsQuery(profile.Country, count);
            var retValTask = await _queryBus.QueryAsync(query, token);
            return retValTask;
        }

        [HttpGet]
        [ResponseCache(Duration = TimeConst.Day, Location = ResponseCacheLocation.Any)]
        public async Task<StatsDto> GetStatsAsync(CancellationToken token)
        {
            var query = new StatsQuery();
            var retValTask = await _queryBus.QueryAsync(query, token);
            return retValTask;
        }


        /// <summary>
        /// Get top document for home page
        /// </summary>
        /// <param name="count">The amount of documents</param>
        /// <param name="urlBuilder">Ignore</param>
        /// <param name="profile">Ignore</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("documents")]
        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = TimeConst.Day, VaryByQueryKeys = new[] { "count" })]

        public async Task<IEnumerable<DocumentFeedDto>> GetTopDocumentsAsync(int count,
            [FromServices] IUrlBuilder urlBuilder,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            var query = new TopDocumentsQuery(profile.Country, count);
            var retValTask = await _queryBus.QueryAsync(query, token);

            return retValTask.Select(s =>
            {
                s.Preview = urlBuilder.BuildDocumentThumbnailEndpoint(s.Id);
                s.Url = Url.DocumentUrl(s.University, s.Course, s.Id, s.Title);
                s.Title = Path.GetFileNameWithoutExtension(s.Title);
                return s;
            });
        }
    }
}
