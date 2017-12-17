using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.DTOs;
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
        private readonly IReadRepositoryAsync<UniversitySynonymDto, long> _universitySynonymRepository;
        private readonly Lazy<IDocumentCseSearch> _searchProvider;
        private readonly Lazy<IFlashcardSearch> _flashcardProvider;
        private readonly Lazy<IQuestionSearch> _questionProvider;

        public SearchController(IReadRepositoryAsync<UniversitySynonymDto, long> universitySynonymRepository,
            Lazy<IDocumentCseSearch> searchProvider, Lazy<IFlashcardSearch> flashcardProvider,
            Lazy<IQuestionSearch> questionProvider)
        {
            _universitySynonymRepository = universitySynonymRepository;
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
        [Route("api/search/documents"), HttpGet]
        public async Task<HttpResponseMessage> SearchDocumentAsync([FromUri] SearchRequest model,
            CancellationToken token)
        {
            if (model.Course != null)
            {
                throw new NotImplementedException();
            }
            IEnumerable<string> universitySynonym = null;
            if (model.University.HasValue)
            {
                var repositoryResult = await _universitySynonymRepository.GetAsync(model.University.Value, token).ConfigureAwait(false);
                universitySynonym = repositoryResult.Name;
            }
            var query = new SearchQuery(model.Query, universitySynonym, null, model.Source, model.Page.GetValueOrDefault(),
                model.Sort.GetValueOrDefault());

            var result = await _searchProvider.Value.SearchAsync(query, token).ConfigureAwait(false);
            return Request.CreateResponse(new
            {
                documents = result.Result,
                result.Facet
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
            if (model.Course != null)
            {
                throw new NotImplementedException();
            }
            IEnumerable<string> universitySynonym = null;
            if (model.University.HasValue)
            {
                var repositoryResult = await _universitySynonymRepository.GetAsync(model.University.Value, token).ConfigureAwait(false);
                universitySynonym = repositoryResult.Name;
            }
            var query = new SearchQuery(model.Query, universitySynonym, null, model.Source, model.Page.GetValueOrDefault(),
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
            IEnumerable<string> universitySynonym = null;
            if (model.University.HasValue)
            {
                var repositoryResult = await _universitySynonymRepository.GetAsync(model.University.Value, token).ConfigureAwait(false);
                universitySynonym = repositoryResult.Name;
            }
            var query = new SearchQuery(model.Query, universitySynonym, null, model.Source, model.Page.GetValueOrDefault(),
                model.Sort.GetValueOrDefault());

            var result = await _questionProvider.Value.SearchAsync(query, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}
