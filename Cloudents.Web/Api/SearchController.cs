using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Search")]
    public class SearchController : Controller
    {
        private readonly IReadRepositoryAsync<UniversitySynonymDto, long> m_UniversitySynonymRepository;

        public SearchController(IReadRepositoryAsync<UniversitySynonymDto, long> universitySynonymRepository)
        {
            m_UniversitySynonymRepository = universitySynonymRepository;
        }

        [Route("documents")]
        public async Task<IActionResult> SearchDocumentAsync([FromQuery] SearchRequest model,
            CancellationToken token, [FromServices] IDocumentCseSearch searchProvider)
        {
            string universitySynonym = null;
            if (model.University.HasValue)
            {
                var repositoryResult = await m_UniversitySynonymRepository.GetAsync(model.University.Value, token).ConfigureAwait(false);
                universitySynonym = repositoryResult.Name;
            }
            var query = new SearchQuery(model.Term, universitySynonym, model.Course, model.Source, model.Page.GetValueOrDefault(),
                model.Sort.GetValueOrDefault());

            var result = await searchProvider.SearchAsync(query, token).ConfigureAwait(false);
            return Json(result);
        }

        [Route("flashcards")]
        public async Task<IActionResult> SearchFlashcardsAsync([FromQuery] SearchRequest model,
            CancellationToken token, [FromServices] IFlashcardSearch searchProvider)
        {
            string universitySynonym = null;
            if (model.University.HasValue)
            {
                var repositoryResult = await m_UniversitySynonymRepository.GetAsync(model.University.Value, token).ConfigureAwait(false);
                universitySynonym = repositoryResult.Name;
            }
            var query = new SearchQuery(model.Term, universitySynonym, model.Course, model.Source, model.Page.GetValueOrDefault(),
                model.Sort.GetValueOrDefault());

            var result = await searchProvider.SearchAsync(query, token).ConfigureAwait(false);
            return Json(result);
        }

        [Route("qna")]
        [Obsolete]
        public async Task<IActionResult> SearchQuestionsAsync([FromQuery] SearchRequest model,
            CancellationToken token, [FromServices] IQuestionSearch searchProvider)
        {
            string universitySynonym = null;
            if (model.University.HasValue && !string.IsNullOrEmpty(model.Course))
            {
                var repositoryResult = await m_UniversitySynonymRepository.GetAsync(model.University.Value, token).ConfigureAwait(false);
                universitySynonym = repositoryResult.Name;
            }
            var query = new SearchQuery(model.Term, universitySynonym, model.Course, model.Source, model.Page.GetValueOrDefault(),
                model.Sort.GetValueOrDefault());

            var result = await searchProvider.SearchAsync(query, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}