﻿using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Query.Feed;
using Cloudents.Core.Query.Feed;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Framework;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.IO;
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
        private readonly IStringLocalizer<DocumentController> _localizer;
        private readonly IUrlBuilder _urlBuilder;
        private readonly IFeedService _feedSort;


        public FeedController( UserManager<User> userManager, 
             IStringLocalizer<DocumentController> localizer, IUrlBuilder urlBuilder,
             IFeedService feedService)
        {
            _userManager = userManager;
            _localizer = localizer;
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

            var result = await _feedSort.GetFeedAsync(new GetFeedQuery(userId, page, request.Filter, profile.Country, null), token);
            return GenerateResult(result, new
            {
                page = ++page,
                filter = request.Filter
            });
        }



        private WebResponseWithFacet<FeedDto> GenerateResult(
            IEnumerable<FeedDto> result, object nextPageParams)
        {
            var filters = new List<IFilters>();

            var filter = new Filters<string>(nameof(DocumentRequestAggregate.Filter),
                _localizer["TypeFilterTitle"],
                EnumExtension.GetValues<FeedType>().Select(s =>
                    new KeyValuePair<string, string>(s.ToString("G"), s.GetEnumLocalization())));
            filters.Add(filter);



            return new WebResponseWithFacet<FeedDto>
            {
                Result = result.Select(s =>
                {
                    if (s is DocumentFeedDto p)
                    {
                        p.Preview = _urlBuilder.BuildDocumentThumbnailEndpoint(p.Id);
                        p.Url = Url.DocumentUrl(p.University, p.Course, p.Id, p.Title);
                        p.Title = Path.GetFileNameWithoutExtension(p.Title);
                    }
                    //TODO add question


                    return s;
                }),
                Filters = filters,
                NextPageLink = Url.RouteUrl("Documents", nextPageParams)
            };
        }

        [HttpGet]
        public async Task<WebResponseWithFacet<FeedDto>> SpecificCourseAsync(
            [RequiredFromQuery]DocumentRequestCourse request,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);

            var result = await _feedSort.GetFeedAsync(new GetFeedQuery(userId, request.Page, request.Filter, profile.Country, request.Course), token);

            return GenerateResult(result, new { page = ++request.Page, request.Course, request.Filter });
        }


        //this is search
        [HttpGet]
        public async Task<WebResponseWithFacet<FeedDto>> SearchInCourseAsync(
            [RequiredFromQuery]  DocumentRequestSearchCourse request,
            [ProfileModelBinder(ProfileServiceQuery.UniversityId | ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            var resultTask = _feedSort.GetFeedAsync(new SearchFeedQuery(profile, request.Term, request.Page, request.Filter, profile.Country, request.Course), token);
            await Task.WhenAll(resultTask);
            
            return GenerateResult(resultTask.Result, new
            {
                page = ++request.Page,
                request.Course,
                request.Term,
                request.Filter
            });
        }

        //this is search

        [HttpGet]
        public async Task<WebResponseWithFacet<FeedDto>> SearchInSpitballAsync(
          [RequiredFromQuery]  DocumentRequestSearch request,

            [ProfileModelBinder(ProfileServiceQuery.UniversityId | ProfileServiceQuery.Country | ProfileServiceQuery.Course)] UserProfile profile,
            CancellationToken token)
        {
            var resultTask = _feedSort.GetFeedAsync(new SearchFeedQuery(profile, request.Term, request.Page, request.Filter, profile.Country, null), token);

            await Task.WhenAll(resultTask);
            return GenerateResult(resultTask.Result, new
            {
                page = ++request.Page,
                request.Term,
                request.Filter
            });
        }

    }
}
