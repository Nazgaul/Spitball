using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
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

        public Task<IEnumerable<SearchResult>> SearchAsync(SearchQuery model, CancellationToken token)
        {
            var cseModel = new SearchModel(model.Query, model.Source, model.Page, model.Sort, CustomApiKey.AskQuestion, null, null, QueryString, null);
            return _search.DoSearchAsync(cseModel, token);
        }
    }
}