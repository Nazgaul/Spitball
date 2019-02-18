using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Request;
using Cloudents.Web.Binders;

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


        public SearchController(IStringLocalizer<SearchController> localizer
        )
        {
            _localizer = localizer;
        }

        /// <summary>
        /// Search flashcard vertical result
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="profile">Profile - generated on server not to transfer</param>
        /// <param name="searchProvider">DI</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("flashcards", Name = "FlashcardSearch"), HttpGet]
        public async Task<WebResponseWithFacet<SearchResult>> SearchFlashcardAsync([FromQuery] SearchRequest model,
            [ProfileModelBinder(ProfileServiceQuery.University)] UserProfile profile,
            [FromServices] IWebFlashcardSearch searchProvider, CancellationToken token)
        {
            var query = BingSearchQuery.Flashcard(model.Query, profile.University?.ExtraName, model.Course, model.Source, model.Page.GetValueOrDefault());
            var result = await searchProvider.SearchWithUniversityAndCoursesAsync(query, token).ConfigureAwait(false);
            string nextPageLink = null;
            var p = result.Result?.ToList();
            if (p?.Any() == true)
            {
                nextPageLink = Url.NextPageLink("FlashcardSearch", null, model);
            }
            return new WebResponseWithFacet<SearchResult>
            {
                Result = p,
                Sort = EnumExtension.GetValues<SearchRequestSort>().Select(s => new KeyValuePair<string, string>(s.ToString("G"), s.GetEnumLocalization())),
                Filters = new IFilters[]
                {
                    new Filters<string>(nameof(SearchRequest.Source),_localizer["Sources"],result.Facet.Select(s=> new KeyValuePair<string, string>(s,s)))
                },
                NextPageLink = nextPageLink
            };
           
        }
    }
}
