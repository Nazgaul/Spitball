using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Models;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class SearchController : ApiController
    {
        private readonly IZboxReadService m_ZboxReadService;

        public SearchController(IZboxReadService zboxReadService)
        {
            m_ZboxReadService = zboxReadService;
        }

        [Route("api/search/documents"), HttpGet]
        public async Task<HttpResponseMessage> SearchDocumentAsync([FromUri]SearchRequest model, CancellationToken token)
        {
            var term = new List<string>();
            if (model.University.HasValue)
            {
                var universitySynonym = await m_ZboxReadService.GetUniversitySynonymAsync(model.University.Value).ConfigureAwait(false);
                term.Add(universitySynonym);
            }

            if (!string.IsNullOrEmpty(model.Course))
            {
                term.Add('"' + model.Course + '"');
            }
            if (model.Query != null)
            {
                term.Add(string.Join(" ", model.Query.Select(s => '"' + s + '"')));
            }

            var result = Enumerable.Range(model.Page * 3, 3).Select(s => DoSearchAsync(string.Join(" ", term), model.Source, s, model.Sort, CustomApiKey.Documents, token)).ToList();
            await Task.WhenAll(result).ConfigureAwait(false);

            //result.Select(s=>s.Result)

            //var t1 = DoSearchAsync(model, universitySynonym, CustomApiKey.Documents, token)
            //var result = await DoSearchAsync(model, universitySynonym, CustomApiKey.Documents, token).ConfigureAwait(false);
            return Request.CreateResponse(new
            {
                documents = result.Where(s => s.Result != null).SelectMany(s => s.Result),
                facet = new[] {
                    "uloop.com",
                    "spitball.co",
                    "studysoup.com",
                    "coursehero.com",
                    "cliffsnotes.com",
                    "oneclass.com",
                    "koofers.com",
                    "studylib.net"
                }
            });
        }

        [Route("api/search/flashcards"), HttpGet]
        public async Task<HttpResponseMessage> SearchFlashcardAsync([FromUri]SearchRequest model, CancellationToken token)
        {
            var term = new List<string>();
            if (model.University.HasValue)
            {
                var universitySynonym = await m_ZboxReadService.GetUniversitySynonymAsync(model.University.Value).ConfigureAwait(false);
                term.Add(universitySynonym);
            }

            if (!string.IsNullOrEmpty(model.Course))
            {
                term.Add('"' + model.Course + '"');
            }
            if (model.Query != null)
            {
                term.Add(string.Join(" ", model.Query.Select(s => '"' + s + '"')));
            }

            var result = Enumerable.Range(model.Page * 3, 3).Select(s => DoSearchAsync(string.Join(" ", term), model.Source, s, model.Sort, CustomApiKey.Flashcard, token)).ToList();
            await Task.WhenAll(result).ConfigureAwait(false);

            //var result = await DoSearchAsync(model, universitySynonym, CustomApiKey.Flashcard, token).ConfigureAwait(false);
            return Request.CreateResponse(new
            {
                documents = result.Where(s => s.Result != null).SelectMany(s => s.Result),
                facet = new[]
                {
                    "quizlet.com",
                    "cram.com",
                    "koofers.com",
                    "coursehero.com",
                    "studysoup.com",
                    "spitball.co"
                }
            });
        }

        [Route("api/search/qna"), HttpGet]
        public async Task<HttpResponseMessage> SearchQuestionAsync([FromUri]SearchRequest model, CancellationToken token)
        {
            var term = new List<string>();
            if (model.University.HasValue)
            {
                var universitySynonym = await m_ZboxReadService.GetUniversitySynonymAsync(model.University.Value).ConfigureAwait(false);
                term.Add(universitySynonym);
            }

            if (!string.IsNullOrEmpty(model.Course))
            {
                term.Add('"' + model.Course + '"');
            }
            if (model.Query != null)
            {
                term.Add(string.Join(" ", model.Query));
            }

            var result = Enumerable.Range(model.Page * 3, 3).Select(s => DoSearchAsync(string.Join(" ", term), model.Source, s, model.Sort, CustomApiKey.AskQuestion, token)).ToList();
            await Task.WhenAll(result).ConfigureAwait(false);

            //var result = await DoSearchAsync(model, universitySynonym, CustomApiKey.AskQuestion, token).ConfigureAwait(false);
            return Request.CreateResponse(result.Where(s => s.Result != null).SelectMany(s => s.Result));
        }

        private static async Task<IEnumerable<SearchResult>> DoSearchAsync(
            string query,
            string source,
            int page,
            // string universitySynonym,
            SearchRequestSort sort,
            CustomApiKey key,
            CancellationToken token)
        {
            var initializer = new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyCZEbkX9Of6pZQ47OD0VA9a8fd1A6IvW6E",

            };

            //if (query.University.HasValue)
            //{
            //    var universitySynonym = await m_ZboxReadService.GetUniversitySynonymAsync(query.University.Value).ConfigureAwait(false);
            //    term.Add(universitySynonym);
            //}
            var p = new CustomsearchService(initializer);

            //int? realPage = null;
            //if (query.Page > 0)
            //{
            //    realPage = query.Page;
            //}
            var request = new CseResource.ListRequest(p, string.Join(" ", query))
            {
                Start = ++page,
                SiteSearch = source,
                Cx = key.Key,
                Fields = "items(title,link,snippet,pagemap/cse_image,displayLink)",
                Sort = sort == SearchRequestSort.Date ? "date" : string.Empty
            };

            var result = await request.ExecuteAsync(token).ConfigureAwait(false);

            var items = result.Items?.Select(s =>
            {
                string image = null;
                if (s.Pagemap != null && s.Pagemap.TryGetValue("cse_image", out var value))
                {
                    if (value[0].TryGetValue("src", out var t))
                    {
                        image = t.ToString();
                    }
                }
                return new SearchResult

                {
                    Id = Md5HashGenerator.GenerateKey(s.Link),
                    Url = s.Link,
                    Title = s.Title,
                    Snippet = s.Snippet,
                    Image = image,
                    Source = s.DisplayLink

                };
            });
            return items;
        }
    }

    public sealed class CustomApiKey
    {
        public string Key { get; }

        public CustomApiKey(string key)
        {
            Key = key;
        }

        public static readonly CustomApiKey Documents = new CustomApiKey("003398766241900814674:a0wbnwb-nyc");
        public static readonly CustomApiKey Flashcard = new CustomApiKey("005511487202570484797:dpt4lypgxoq");
        public static readonly CustomApiKey AskQuestion = new CustomApiKey("partner-pub-1215688692145777:pwibhr1onij");
    }
}
