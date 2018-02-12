using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Mobile.Extensions;
using Cloudents.Mobile.Models;
using Microsoft.Azure.Mobile.Server.Config;

namespace Cloudents.Mobile.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Search Cse controller for flashcard and document
    /// </summary>
    [MobileAppController]
    public class SearchController : ApiController
    {
        private readonly Lazy<IDocumentCseSearch> _searchProvider;
        private readonly Lazy<IFlashcardSearch> _flashcardProvider;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="searchProvider"></param>
        /// <param name="flashcardProvider"></param>
        public SearchController(
            Lazy<IDocumentCseSearch> searchProvider, Lazy<IFlashcardSearch> flashcardProvider
            )
        {
            _searchProvider = searchProvider;
            _flashcardProvider = flashcardProvider;
        }

        /// <summary>
        /// Search document vertical result
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/search/documents", Name = "DocumentSearch"), HttpGet]
        public async Task<IHttpActionResult> SearchDocumentAsync([FromUri] SearchRequest model,
            CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetError());
            }
            var query = SearchQuery.Document(model.Query, model.University, model.Course, model.Source, model.Page.GetValueOrDefault(),
                model.Sort.GetValueOrDefault(), model.DocType);
            var result = await _searchProvider.Value.SearchAsync(query, model.Format, token).ConfigureAwait(false);

            string nextPageLink = null;
            if (result.Result?.Any() == true)
            {
                nextPageLink = Url.NextPageLink("DocumentSearch", null, model);
            }

            return Ok(new
            {
                documents = result.Result,
                result.Facet,
                nextPageLink
            });
        }

        /// <summary>
        /// Search flashcard vertical result
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/search/flashcards", Name = "FlashcardSearch"), HttpGet]
        public async Task<IHttpActionResult> SearchFlashcardAsync([FromUri] SearchRequest model,
            CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetError());
            }
            var query = SearchQuery.Flashcard(model.Query, model.University, model.Course, model.Source, model.Page.GetValueOrDefault(),
                model.Sort.GetValueOrDefault());

            var result = await _flashcardProvider.Value.SearchAsync(query, model.Format, token).ConfigureAwait(false);
            string nextPageLink = null;
            if (result.Result?.Any() == true)
            {
                nextPageLink = Url.NextPageLink("FlashcardSearch", null, model);
            }
            return Ok(new
            {
                documents = result.Result,
                result.Facet,
                nextPageLink
            });
        }
    }
}
