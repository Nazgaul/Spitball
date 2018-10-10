using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Core.Request;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Search Cse controller for flashcard and document
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IStringLocalizer<SearchController> _localizer;
        private readonly SignInManager<User> _signInManager;
        private readonly IQueryBus _queryBus;


        public SearchController(IStringLocalizer<SearchController> localizer, SignInManager<User> signInManager, IQueryBus queryBus)
        {
            _localizer = localizer;
            _signInManager = signInManager;
            _queryBus = queryBus;
        }

        /// <summary>
        /// Search document vertical result
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ilSearchProvider"></param>
        /// <param name="token"></param>
        /// <param name="universityId">This params comes from claim and not from api - value is ignored</param>
        /// <param name="country">This params comes from server internal process - value is ignored</param>
        /// <param name="searchProvider"></param>
        /// <returns></returns>
        [Route("documents", Name = "DocumentSearch"), HttpGet]
        public async Task<WebResponseWithFacet<SearchResult>> SearchDocumentAsync([FromQuery] SearchRequest model,
           [ClaimModelBinder(AppClaimsPrincipalFactory.University)] long? universityId,
            [ModelBinder(typeof(CountryModelBinder))] string country,
           [FromServices] IWebDocumentSearch searchProvider,
            [FromServices] IDocumentSearch ilSearchProvider,
            CancellationToken token)
        {

            model = model ?? new SearchRequest();

            var coursesTask = Task.FromResult<IEnumerable<CourseDto>>(null);

            if (_signInManager.IsSignedIn(User))
            {
                var userId = _signInManager.UserManager.GetLongUserId(User);
                var dbQuery = new CoursesQuery(userId);
                coursesTask = _queryBus.QueryAsync(dbQuery, token);
            }
            var query = SearchQuery.Document(model.Query, universityId, model.Course, model.Source, model.Page.GetValueOrDefault());

            Task<ResultWithFacetDto<SearchResult>> resultTask;
            if (string.Equals(country, "il", StringComparison.OrdinalIgnoreCase))
            {
                resultTask = ilSearchProvider.SearchDocumentsAsync(query, token);
            }

            else
            {

                resultTask = searchProvider.SearchWithUniversityAndCoursesAsync(query, model.Format, token);
            }

            await Task.WhenAll(coursesTask, resultTask);
            var result = resultTask.Result;
            var p = result.Result?.ToList();
            string nextPageLink = null;
            if (p?.Any() == true)
            {
                nextPageLink = Url.NextPageLink("DocumentSearch", null, model);
            }
            var filters = new List<IFilters>();

            if (result.Facet != null)
            {
                filters.Add( 
                
                    new Filters<string>(nameof(SearchRequest.Source), _localizer["Sources"],
                        result.Facet.Select(s => new KeyValuePair<string, string>(s, s)))

                );
            }

            if (coursesTask.Result != null)
            {
                filters.Add(new Filters<long>(nameof(SearchRequest.Course),
                    _localizer["CoursesFilterTitle"],
                    coursesTask.Result.Select(s => new KeyValuePair<long, string>(s.Id, s.Name))));
            }
            return new WebResponseWithFacet<SearchResult>
            {
                Result = p,
                Sort = Enum.GetNames(typeof(SearchRequestSort)).Select(s => new KeyValuePair<string, string>(s, s)),
                Filters = filters,
                NextPageLink = nextPageLink
            };




        }

       


        /// <summary>
        /// Search flashcard vertical result
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="searchProvider"></param>
        /// <param name="universityId">This params comes from claim and not from api - value is ignored</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("flashcards", Name = "FlashcardSearch"), HttpGet]
        public async Task<WebResponseWithFacet<SearchResult>> SearchFlashcardAsync([FromQuery] SearchRequest model,
            [ClaimModelBinder(AppClaimsPrincipalFactory.University)] long? universityId,
            [FromServices] IWebFlashcardSearch searchProvider, CancellationToken token)
        {
            var query = SearchQuery.Flashcard(model.Query, universityId, model.Course, model.Source, model.Page.GetValueOrDefault());
            var result = await searchProvider.SearchWithUniversityAndCoursesAsync(query, model.Format, token).ConfigureAwait(false);
            string nextPageLink = null;
            var p = result.Result?.ToList();
            if (p?.Any() == true)
            {
                nextPageLink = Url.NextPageLink("FlashcardSearch", null, model);
            }
            return new WebResponseWithFacet<SearchResult>
            {
                Result = p,
                Sort = Enum.GetNames(typeof(SearchRequestSort)).Select(s => new KeyValuePair<string, string>(s, s)),
                Filters = new IFilters[]
                {
                    new Filters<string>(nameof(SearchRequest.Source),_localizer["Sources"],result.Facet.Select(s=> new KeyValuePair<string, string>(s,s)))
                },
                //Facet = result.Facet,
                NextPageLink = nextPageLink
            };

            //return new WebResponseWithFacet<SearchResult>
            //{
            //    Result = p,
            //    Sort = Enum.GetNames(typeof(SearchRequestSort)),
            //    Filters = new Dictionary<string, IEnumerable<string>>
            //    {
            //        ["Sources"] = result.Facet
            //    },
            //    NextPageLink = nextPageLink
            //};
        }
    }
}
