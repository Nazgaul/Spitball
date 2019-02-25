using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;

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
        //private readonly IStringLocalizer<SearchController> _localizer;


        //public SearchController(IStringLocalizer<SearchController> localizer
        //)
        //{
        //    _localizer = localizer;
        //}

        /// <summary>
        /// Search flashcard vertical result
        /// </summary>
        /// <returns>None - this atm not supported</returns>
        [Route("flashcards", Name = "FlashcardSearch"), HttpGet]
        public WebResponseWithFacet<SearchResult> SearchFlashcardAsync(
           // [FromQuery] SearchRequest model,
            //[ProfileModelBinder(ProfileServiceQuery.University)] UserProfile profile,
           //[FromServices] IWebFlashcardSearch searchProvider, CancellationToken token
           )
        {
           // var query = BingSearchQuery.Flashcard(model.Query, profile.University?.ExtraName, model.Course, model.Source, model.Page.GetValueOrDefault());
            //var result = await searchProvider.SearchWithUniversityAndCoursesAsync(query, token);
            //string nextPageLink = null;
           // var p = result.Result?.ToList();
           // if (p?.Any() == true)
           // {
           //     nextPageLink = Url.NextPageLink("FlashcardSearch", null, model);
           // }
            return new WebResponseWithFacet<SearchResult>
            {
                Result = new SearchResult[0],
                Sort = EnumExtension.GetValues<SearchRequestSort>().Select(s => new KeyValuePair<string, string>(s.ToString("G"), s.GetEnumLocalization())),
                Filters = new IFilters[]
                {
                    //new Filters<string>(nameof(SearchRequest.Source),_localizer["Sources"],result.Facet.Select(s=> new KeyValuePair<string, string>(s,s)))
                },
                NextPageLink = null
            };
           
        }
    }
}
