﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.MobileApi.Extensions;
using Cloudents.MobileApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.MobileApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Search Cse controller for flashcard and document
    /// </summary>
    [Route("api/[controller]")]
    public class SearchController : Controller
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
        [Route("documents", Name = "DocumentSearch"), HttpGet]
        public async Task<IActionResult> SearchDocumentAsync([FromQuery] SearchRequest model,
            CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var query = SearchQuery.Document(model.Query, model.University, model.Course, model.Source, model.Page.GetValueOrDefault(),
                model.Sort.GetValueOrDefault(), model.DocType);
            var result = await _searchProvider.Value.SearchAsync(query, model.Format, token).ConfigureAwait(false);

            var resultList = result.Result.ToListIgnoreNull();
            string nextPageLink = null;
            if (resultList.Count > 0)
            {
                nextPageLink = Url.NextPageLink("DocumentSearch", null, model);
            }

            return Ok(new
            {
                result = resultList,
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
        [Route("flashcards", Name = "FlashcardSearch"), HttpGet]
        public async Task<IActionResult> SearchFlashcardAsync([FromQuery] SearchRequest model,
            CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
                result = result.Result,
                result.Facet,
                nextPageLink
            });
        }
    }
}
