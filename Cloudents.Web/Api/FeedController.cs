﻿using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.DTOs.Feed;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Query.Feed;
using Cloudents.Query;
using Cloudents.Query.Users;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Framework;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUrlBuilder _urlBuilder;
        private readonly IFeedService _feedService;


        public FeedController(UserManager<User> userManager,
              IUrlBuilder urlBuilder,
             IFeedService feedService)
        {
            _userManager = userManager;
            _urlBuilder = urlBuilder;
            _feedService = feedService;
        }


        [HttpGet(Name = "Documents")]
        public async Task<WebResponseWithFacet<FeedDto>> AggregateAllCoursesAsync(
         [FromQuery]DocumentRequestAggregate request,
         [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
         CancellationToken token)
        {
            var page = request.Page;

            _userManager.TryGetLongUserId(User, out var userId);

            var result = await _feedService.GetFeedAsync(new GetFeedQuery(userId, page, request.Filter, profile.CountryRegion), token);
            return GenerateResult(result,
                new List<string>()
                {
                    FeedType.Document.ToString("G"),
                    FeedType.Video.ToString("G"),
                    FeedType.Question.ToString("G"),
                    FeedType.Tutor.ToString("G")
                });
        }



        private WebResponseWithFacet<FeedDto> GenerateResult(
            IEnumerable<FeedDto> result, IEnumerable<string> filters)
        {




            return new WebResponseWithFacet<FeedDto>
            {
                Result = result.Select(s =>
                {
                    if (s is DocumentFeedDto p)
                    {
                        p.Preview = _urlBuilder.BuildDocumentThumbnailEndpoint(p.Id);
                        p.Url = Url.DocumentUrl(p.Course, p.Id, p.Title);
                    }
                    return s;
                }),
                Filters = filters
            };
        }

        [HttpGet]
        public async Task<WebResponseWithFacet<FeedDto>> SpecificCourseAsync(
            [RequiredFromQuery]DocumentRequestCourse request,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);

            var result = await _feedService.GetFeedAsync(new GetFeedWithCourseQuery(userId, request.Page, request.Filter, profile.CountryRegion, request.Course), token);

            return GenerateResult(result,
                new List<string>()
                {
                    FeedType.Document.ToString("G"),
                    FeedType.Video.ToString("G"),
                    FeedType.Question.ToString("G"),
                    FeedType.Tutor.ToString("G")
                });
        }


        //this is search
        [HttpGet]
        public async Task<WebResponseWithFacet<FeedDto>> SearchInCourseAsync(
            [RequiredFromQuery]  DocumentRequestSearchCourse request,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            var result = await _feedService.GetFeedAsync(new SearchFeedQuery(profile, 
                request.Term, request.Page, request.Filter,  request.Course), token);

            return GenerateResult(result,
                new List<string>()
                {
                    FeedType.Document.ToString("G"),
                    FeedType.Video.ToString("G"),
                    FeedType.Tutor.ToString("G")
                });
        }

        //this is search

        [HttpGet]
        public async Task<WebResponseWithFacet<FeedDto>> SearchInSpitballAsync(
            [RequiredFromQuery]  DocumentRequestSearch request,
            [ProfileModelBinder(ProfileServiceQuery.Country | ProfileServiceQuery.Course)] UserProfile profile,
            CancellationToken token)
        {
            var result = await _feedService.GetFeedAsync(new SearchFeedQuery(profile, 
                request.Term, request.Page, request.Filter,  null), token);
            return GenerateResult(result,
                new List<string>()
                {
                    FeedType.Document.ToString("G"),
                    FeedType.Video.ToString("G"),
                    FeedType.Tutor.ToString("G")
                });
        }

        [HttpGet("courses")]
        // [Authorize]
        public async Task<IEnumerable<string>> GetUserCoursesAsync([FromServices] IQueryBus queryBus, CancellationToken token)
        {
            if (!_userManager.TryGetLongUserId(User, out var userId))
            {
                return Enumerable.Empty<string>();
            }
            var query = new UserCoursesNamesQuery(userId);
            return await queryBus.QueryAsync(query, token);
        }

    }
}
