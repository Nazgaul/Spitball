using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        public async Task<HttpResponseMessage> SearchDocumentAsync([FromUri]SearchRequest model)
        {
            var result = await DoSearchAsync(model, CustomApiKey.Documents).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }

        [Route("api/search/flashcards"), HttpGet]
        public async Task<HttpResponseMessage> SearchFlashcardAsync([FromUri]SearchRequest model)
        {
            var result = await DoSearchAsync(model, CustomApiKey.Flashcard).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }

        [Route("api/search/qna"), HttpGet]
        public async Task<HttpResponseMessage> SearchQuestionAsync([FromUri]SearchRequest model)
        {
            var result = await DoSearchAsync(model, CustomApiKey.AskQuestion).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }


        private async Task<IEnumerable<SearchResult>> DoSearchAsync(SearchRequest query, CustomApiKey key)
        {
            var initializer = new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyCZEbkX9Of6pZQ47OD0VA9a8fd1A6IvW6E",

            };
            var term = new List<string>()
            {

                query.Course?.Replace(" ", "+")
            };
            if (query.Query != null)
            {
                term.Add(string.Join("+", query.Query.Select(s => '"' + s + '"')));
            }
            if (query.University.HasValue)
            {

                var universitySynonym = await m_ZboxReadService.GetUniversitySynonymAsync(query.University.Value).ConfigureAwait(false);
                term.Add(universitySynonym);
            }
            var p = new CustomsearchService(initializer);

            int? realPage = null;
            if (query.Page > 0)
            {
                realPage = query.Page;
            }
            var request = new CseResource.ListRequest(p, string.Join(" ", term))
            {
                Start = realPage,
                SiteSearch = query.Source,
                Cx = key.Key,
                Fields = "items(title,link,snippet,pagemap/cse_image,displayLink)"
            };


            var result = await request.ExecuteAsync().ConfigureAwait(false);

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
