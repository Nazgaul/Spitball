using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Query;
using Cloudents.Query.HomePage;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            var query = new TopTutorsQuery(profile.CountryRegion, count);
            var result = await _queryBus.QueryAsync(query, token);
            return result.Select(s =>
            {
                s.Image = _urlBuilder.BuildUserImageEndpoint(s.UserId, s.Image);
                return s;
            });
           
        }

        /// <summary>
        /// Get banner for home page
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("banner")]
        public async Task<BannerDto> GetTopBannerAsync(CancellationToken token)
        {
            var query = new GetBannerQuery(CultureInfo.CurrentCulture);
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
            var query = new TopDocumentsQuery(profile.CountryRegion, count);
            var result = await _queryBus.QueryAsync(query, token);

            return result.Select(item =>
            {
                item.User.Image = _urlBuilder.BuildUserImageEndpoint(item.User.Id, item.User.Image);
                item.Preview = urlBuilder.BuildDocumentThumbnailEndpoint(item.Id);
                item.Url = Url.DocumentUrl(item.Course, item.Id, item.Title);
                return item;
            });
           
        }
    }
}
