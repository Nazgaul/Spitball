using System;
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
    [MobileAppController]
    public class SearchController : ApiController
    {
        private readonly IReadRepositorySingle<UniversitySynonymDto, long> m_UniversitySynonymRepository;
        private readonly Lazy<IDocumentSearch> m_SearchProvider;
        private readonly Lazy<IFlashcardSearch> m_FlashcardProvider;
        private readonly Lazy<IQuestionSearch> m_QuestionProvider;

        public SearchController(IReadRepositorySingle<UniversitySynonymDto, long> universitySynonymRepository,
            Lazy<IDocumentSearch> searchProvider, Lazy<IFlashcardSearch> flashcardProvider,
            Lazy<IQuestionSearch> questionProvider)
        {
            m_UniversitySynonymRepository = universitySynonymRepository;
            m_SearchProvider = searchProvider;
            m_FlashcardProvider = flashcardProvider;
            m_QuestionProvider = questionProvider;
        }

        [Route("api/search/documents"), HttpGet]
        public async Task<HttpResponseMessage> SearchDocumentAsync([FromUri] SearchRequest model,
            CancellationToken token)
        {
            string universitySynonym = null;
            if (model.University.HasValue)
            {
                var repositoryResult = await m_UniversitySynonymRepository.GetAsync(model.University.Value, token).ConfigureAwait(false);
                universitySynonym = repositoryResult.Name;
            }
            var query = new SearchQuery(model.Query, universitySynonym, model.Course, model.Source, model.Page,
                model.Sort);

            var result = await m_SearchProvider.Value.SearchAsync(query, token).ConfigureAwait(false);
            return Request.CreateResponse(new
            {
               documents =  result.Result,
               result.Facet
            });
        }

        [Route("api/search/flashcards"), HttpGet]
        public async Task<HttpResponseMessage> SearchFlashcardAsync([FromUri] SearchRequest model,
            CancellationToken token)
        {
            string universitySynonym = null;
            if (model.University.HasValue)
            {
                var repositoryResult = await m_UniversitySynonymRepository.GetAsync(model.University.Value, token).ConfigureAwait(false);
                universitySynonym = repositoryResult.Name;
            }
            var query = new SearchQuery(model.Query, universitySynonym, model.Course, model.Source, model.Page,
                model.Sort);

            var result = await m_FlashcardProvider.Value.SearchAsync(query, token).ConfigureAwait(false);
            return Request.CreateResponse(new
            {
                documents = result.Result,
                result.Facet
            });
        }

        [Route("api/search/qna"), HttpGet]
        public async Task<HttpResponseMessage> SearchQuestionAsync([FromUri] SearchRequest model,
            CancellationToken token)
        {
            string universitySynonym = null;
            if (model.University.HasValue && !string.IsNullOrEmpty(model.Course))
            {
                var repositoryResult = await m_UniversitySynonymRepository.GetAsync(model.University.Value, token).ConfigureAwait(false);
                universitySynonym = repositoryResult.Name;
            }
            var query = new SearchQuery(model.Query, universitySynonym, model.Course, model.Source, model.Page,
                model.Sort);

            var result = await m_QuestionProvider.Value.SearchAsync(query, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}
