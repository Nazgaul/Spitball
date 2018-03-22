using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Api.Extensions;
using Cloudents.Api.Filters;
using Cloudents.Api.Models;
using Cloudents.Core.DTOs;
using Cloudents.Core.Read;
using Cloudents.Core.Request;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Api.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Search Cse controller for flashcard and document
    /// </summary>
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private readonly WebSearch.Factory _factory;

        public SearchController(WebSearch.Factory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Search document vertical result
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(WebResponseWithFacet<SearchResult>), 200)]
        [Route("documents", Name = "DocumentSearch"), HttpGet, ValidateModel]
        public async Task<IActionResult> SearchDocumentAsync([FromQuery] SearchRequest model,
            CancellationToken token)
        {
            var query = SearchQuery.Document(model.Query, model.University, model.Course, model.Source, model.Page.GetValueOrDefault(),
                 model.DocType, model.GeoPoint.ToGeoPoint());
            var searchProvider = _factory.Invoke(CustomApiKey.Documents);
            var result = await searchProvider.SearchWithUniversityAndCoursesAsync(query, model.Format, token).ConfigureAwait(false);

            var p = result.Result?.ToList();
            string nextPageLink = null;
            if (p?.Any() == true)
            {
                nextPageLink = Url.NextPageLink("DocumentSearch", null, model);
            }

            return Ok(new WebResponseWithFacet<SearchResult>
            {
                Result = p,
                Facet = result.Facet,
                NextPageLink = nextPageLink
            });
        }

        /// <summary>
        /// Search flashcard vertical result
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("flashcards", Name = "FlashcardSearch"), HttpGet, ValidateModel]
        [ProducesResponseType(typeof(WebResponseWithFacet<SearchResult>), 200)]

        public async Task<IActionResult> SearchFlashcardAsync([FromQuery] SearchRequest model,
            CancellationToken token)
        {
            var query = SearchQuery.Flashcard(model.Query, model.University, model.Course, model.Source, model.Page.GetValueOrDefault(), model.GeoPoint.ToGeoPoint());
            var searchProvider = _factory.Invoke(CustomApiKey.Flashcard);
            var result = await searchProvider.SearchWithUniversityAndCoursesAsync(query, model.Format, token).ConfigureAwait(false);
            string nextPageLink = null;
            var p = result.Result?.ToList();
            if (p?.Any() == true)
            {
                nextPageLink = Url.NextPageLink("FlashcardSearch", null, model);
            }
            return Ok(new WebResponseWithFacet<SearchResult>
            {
                Result = p,
                Facet = result.Facet,
                NextPageLink = nextPageLink
            });
        }
    }
}
