﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Models;

namespace Zbang.Cloudents.Jared.Controllers
{
    /// <summary>
    /// Search Cse controller for flashcard and document
    /// </summary>
    [MobileAppController]
    public class SearchController : ApiController
    {
        private readonly Lazy<IDocumentCseSearch> _searchProvider;
        private readonly Lazy<IFlashcardSearch> _flashcardProvider;
        private readonly Lazy<IQuestionSearch> _questionProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="searchProvider"></param>
        /// <param name="flashcardProvider"></param>
        /// <param name="questionProvider"></param>
        public SearchController(
            Lazy<IDocumentCseSearch> searchProvider, Lazy<IFlashcardSearch> flashcardProvider,
            Lazy<IQuestionSearch> questionProvider)
        {
            _searchProvider = searchProvider;
            _flashcardProvider = flashcardProvider;
            _questionProvider = questionProvider;
        }

        /// <summary>
        /// Search document vertical result
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/search/documents",Name = "DocumentSearch"), HttpGet]
        public async Task<HttpResponseMessage> SearchDocumentAsync([FromUri] SearchRequest model,
            CancellationToken token)
        {
            var query = new SearchQuery(model.Query, model.University, model.Course, model.Source, model.Page.GetValueOrDefault(),
                model.Sort.GetValueOrDefault());
            var result = await _searchProvider.Value.SearchAsync(query, token).ConfigureAwait(false);

            //https://stackoverflow.com/questions/8391055/passing-an-array-to-routevalues-and-have-it-render-model-binder-friendly-url
            //var nextPageLink = Url.Link("DocumentSearch",
            //    model.GetNextPage());
            return Request.CreateResponse(new
            {
                documents = result.Result,
                result.Facet,
                //nextPageLink
            });
        }

        /// <summary>
        /// Search flashcard vertical result
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/search/flashcards"), HttpGet]
        public async Task<HttpResponseMessage> SearchFlashcardAsync([FromUri] SearchRequest model,
            CancellationToken token)
        {
            var query = new SearchQuery(model.Query, model.University, model.Course, model.Source, model.Page.GetValueOrDefault(),
                model.Sort.GetValueOrDefault());

            var result = await _flashcardProvider.Value.SearchAsync(query, token).ConfigureAwait(false);
            return Request.CreateResponse(new
            {
                documents = result.Result,
                result.Facet
            });
        }

        [Route("api/search/qna"), HttpGet, Obsolete]
        public async Task<HttpResponseMessage> SearchQuestionAsync([FromUri] SearchRequest model,
            CancellationToken token)
        {
            var query = new SearchQuery(model.Query,  model.Page.GetValueOrDefault());
            var result = await _questionProvider.Value.SearchAsync(query, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}
