using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Cloudents.Query;
using Cloudents.Query.Documents;
using Cloudents.Query.Query;
using Cloudents.Query.Tutor;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Framework;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<DocumentController> _localizer;
        private readonly IUrlBuilder _urlBuilder;
        private readonly IFeedSort _feedSort;


        public FeedController(IQueryBus queryBus, UserManager<User> userManager, 
             IStringLocalizer<DocumentController> localizer, IUrlBuilder urlBuilder, IFeedSort feedSort)
        {
            _queryBus = queryBus;
            _userManager = userManager;
            _localizer = localizer;
            _urlBuilder = urlBuilder;
            _feedSort = feedSort;
        }


        [HttpGet(Name = "Documents")]
        public async Task<WebResponseWithFacet<FeedDto>> AggregateAllCoursesAsync(
         [FromQuery]DocumentRequestAggregate request,
         [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
         CancellationToken token)
        {
            var page = request.Page;

            _userManager.TryGetLongUserId(User, out var userId);

            var query = new FeedAggregateQuery(userId, page, request.Filter, profile.Country,null);
            var tutorQuery = new TutorListQuery(userId, profile.Country, page, 3);
            var itemsTask = _queryBus.QueryAsync(query, token);
            var tutorsTask = _queryBus.QueryAsync(tutorQuery, token);
            await Task.WhenAll(itemsTask, tutorsTask);
            var result = _feedSort.SortFeed(itemsTask.Result.ToList(), tutorsTask.Result.ToList(), request.Page);
            //var result = itemsTask.Result.Concat(tutorsTask.Result);
            return GenerateResult(result, new
            {
                page = ++page,
                filter = request.Filter
            });
        }
       
        
        private WebResponseWithFacet<FeedDto> GenerateResult(
            IEnumerable<FeedDto> result, object nextPageParams)
        {
            //string nextPageLink = null;
            //if (p.Count > 0)
            //{
            //    nextPageLink = Url.RouteUrl("Documents", nextPageParams);
            //}

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
            var query = new FeedAggregateQuery(userId, request.Page, request.Filter, profile.Country, request.Course);
            var tutorQuery = new TutorListByCourseQuery(request.Course, userId, profile.Country, 3, request.Page);
            var itemsTask = _queryBus.QueryAsync(query, token);
            var tutorsTask = _queryBus.QueryAsync(tutorQuery, token);
            await Task.WhenAll(itemsTask, tutorsTask);
           
            var result = _feedSort.SortFeed(itemsTask.Result.ToList(), tutorsTask.Result.ToList(), request.Page);

            return GenerateResult(result, new { page = ++request.Page, request.Course, request.Filter });
        }


        //this is search
        [HttpGet]
        public async Task<WebResponseWithFacet<FeedDto>> SearchInCourseAsync(
            [RequiredFromQuery]  DocumentRequestSearchCourse request,
            [ProfileModelBinder(ProfileServiceQuery.UniversityId | ProfileServiceQuery.Country)] UserProfile profile,
            [FromServices] IDocumentSearch searchProvider,
            [FromServices] ITutorSearch tutorSearch,
            CancellationToken token)
        {

      
            var term = $"{request.Term} {request.Course}".Trim();

            var query = new DocumentQuery(profile, request.Term, request.Course,  request.Filter?.Where(w => !string.IsNullOrEmpty(w)))
            {
                Page = request.Page,
            };

            /**/
            var tutorQuery = new TutorListTabSearchQuery(term, profile.Country, request.Page, 3);
            var tutorTask = tutorSearch.SearchAsync(tutorQuery, token);
            var resultTask = searchProvider.SearchDocumentsAsync(query, token);
            var votesTask = Task.FromResult<Dictionary<long, VoteType>>(null);

            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetLongUserId(User);
                var queryTags = new UserVotesByCategoryQuery(userId);
                votesTask = _queryBus.QueryAsync(queryTags, token)
                    .ContinueWith(
                    t2 =>
                    {
                        return t2.Result.ToDictionary(x => x.Id, s => s.Vote);
                    }, token);

            }

            await Task.WhenAll(resultTask, votesTask, tutorTask);
            var result = _feedSort.SortFeed(resultTask.Result.ToList(), tutorTask.Result.ToList(), request.Page);
            return GenerateResult(result, new
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
            [FromServices] IDocumentSearch searchProvider,
            [FromServices] ITutorSearch tutorSearch,
            CancellationToken token)
        {
            var term = request.Term.Trim();

            //Task<IEnumerable<TutorCardDto>> tutorTask = null;
            var query = new DocumentQuery(profile, request.Term, null,
                 request.Filter?.Where(w => !string.IsNullOrEmpty(w)))
            {
                Page = request.Page,
            };

            var tutorQuery = new TutorListTabSearchQuery(term, profile.Country, request.Page, 3);
            var tutorTask = tutorSearch.SearchAsync(tutorQuery, token);

            var resultTask = searchProvider.SearchDocumentsAsync(query, token);
            var votesTask = Task.FromResult<Dictionary<long, VoteType>>(null);

            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetLongUserId(User);
                var queryTags = new UserVotesByCategoryQuery(userId);
                votesTask = _queryBus.QueryAsync(queryTags, token)
                    .ContinueWith(
                        t2 =>
                        {
                            return t2.Result.ToDictionary(x => x.Id, s => s.Vote);
                        }, token);

            }

            await Task.WhenAll(resultTask, votesTask, tutorTask);
            var result = _feedSort.SortFeed(resultTask.Result.ToList(), tutorTask.Result.ToList(), request.Page);
            return GenerateResult(result, new
            {
                page = ++request.Page,
                request.Term,
                request.Filter
            });
        }

    }
}
