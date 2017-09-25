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

            var result = m_SearchProvider.Value.SearchAsync(query, token);
            return Request.CreateResponse(result);

            //var term = new List<string>();
            //if (model.University.HasValue)
            //{
            //    var universitySynonym = await m_UniversitySynonymRepository.GetAsync(model.University.Value, token)
            //        .ConfigureAwait(false);
            //    term.Add(universitySynonym.Name);
            //}

            //if (!string.IsNullOrEmpty(model.Course))
            //{
            //    term.Add('"' + model.Course + '"');
            //}
            //if (model.Query != null)
            //{
            //    term.Add(string.Join(" ", model.Query.Select(s => '"' + s + '"')));
            //}

            //var result = Enumerable.Range(model.Page * 3, 3).Select(s =>
            //        DoSearchAsync(string.Join(" ", term), model.Source, s, model.Sort, CustomApiKey.Documents, token))
            //    .ToList();
            //await Task.WhenAll(result).ConfigureAwait(false);

            ////result.Select(s=>s.Result)

            ////var t1 = DoSearchAsync(model, universitySynonym, CustomApiKey.Documents, token)
            ////var result = await DoSearchAsync(model, universitySynonym, CustomApiKey.Documents, token).ConfigureAwait(false);
            //return Request.CreateResponse(new
            //{
            //    documents = result.Where(s => s.Result != null).SelectMany(s => s.Result),
            //    facet = new[]
            //    {
            //        "uloop.com",
            //        "spitball.co",
            //        "studysoup.com",
            //        "coursehero.com",
            //        "cliffsnotes.com",
            //        "oneclass.com",
            //        "koofers.com",
            //        "studylib.net"
            //    }
            //});
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

            var result = m_FlashcardProvider.Value.SearchAsync(query, token);
            return Request.CreateResponse(result);


            //var term = new List<string>();
            //if (model.University.HasValue)
            //{
            //    var universitySynonym = await m_UniversitySynonymRepository.GetAsync(model.University.Value, token)
            //        .ConfigureAwait(false);
            //    term.Add(universitySynonym.Name);
            //}

            //if (!string.IsNullOrEmpty(model.Course))
            //{
            //    term.Add('"' + model.Course + '"');
            //}
            //if (model.Query != null)
            //{
            //    term.Add(string.Join(" ", model.Query.Select(s => '"' + s + '"')));
            //}

            //var result = Enumerable.Range(model.Page * 3, 3).Select(s =>
            //        DoSearchAsync(string.Join(" ", term), model.Source, s, model.Sort, CustomApiKey.Flashcard, token))
            //    .ToList();
            //await Task.WhenAll(result).ConfigureAwait(false);

            ////var result = await DoSearchAsync(model, universitySynonym, CustomApiKey.Flashcard, token).ConfigureAwait(false);
            //return Request.CreateResponse(new
            //{
            //    documents = result.Where(s => s.Result != null).SelectMany(s => s.Result),
            //    facet = new[]
            //    {
            //        "quizlet.com",
            //        "cram.com",
            //        "koofers.com",
            //        "coursehero.com",
            //        "studysoup.com",
            //        "spitball.co"
            //    }
            //});
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

            var result = m_QuestionProvider.Value.SearchAsync(query, token);
            return Request.CreateResponse(result);


            //var term = new List<string>();

            //if (model.University.HasValue && !string.IsNullOrEmpty(model.Course))
            //{
            //    var universitySynonym = await m_UniversitySynonymRepository.GetAsync(model.University.Value, token)
            //        .ConfigureAwait(false);
            //    term.Add(universitySynonym.Name);
            //}

            //if (!string.IsNullOrEmpty(model.Course))
            //{
            //    term.Add('"' + model.Course + '"');
            //}
            //if (model.Query != null)
            //{
            //    term.Add(string.Join(" ", model.Query));
            //}

            //var result = Enumerable.Range(model.Page * 3, 3).Select(s =>
            //        DoSearchAsync(string.Join(" ", term), model.Source, s, model.Sort, CustomApiKey.AskQuestion, token))
            //    .ToList();
            //await Task.WhenAll(result).ConfigureAwait(false);
            //return Request.CreateResponse(result.Where(s => s.Result != null).SelectMany(s => s.Result));
        }

    }
}
