using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;

namespace Cloudents.Infrastructure.Search
{
    public class QuestionSearch : IQuestionSearch
    {
        public const string QueryString = "world war 2";
        private readonly ISearch _search;

        public QuestionSearch(ISearch search)
        {
            _search = search;
        }

        public async Task<ResultWithFacetDto<SearchResult>> SearchAsync(SearchQuery model, BingTextFormat format, CancellationToken token)
        {
            var cseModel = new SearchModel(model.Query, model.Source,  model.Sort, CustomApiKey.AskQuestion, null, null, QueryString, null);
            var result = await _search.DoSearchAsync(cseModel, model.Page, format, token).ConfigureAwait(false);
            return new ResultWithFacetDto<SearchResult>
            {
                Result = result,
                Facet = new[]
                {
                    "khanacademy.org",
                    "yalescientific.org",
                    "worldatlas.com",
                    "wired.com",
                    "wikihow.com",
                    "thoughtco.com",
                    "space.com",
                    "snapguide.com",
                    "simple.wikipedia.org",
                    "reference.com",
                    "physics.org",
                    "quora.com",
                    "newworldencyclopedia.org",
                    "lumenlearning.com",
                    "livescience.com",
                    "knowledgedoor.com",
                    "howstuffworks.com",
                    "history.com",
                    "enotes.com",
                    "encyclopedia.com",
                    "businessinsider.com",
                    "britannica.com",
                    "boundless.com",
                    "socratic.org"
                }
            };
        }
    }
}