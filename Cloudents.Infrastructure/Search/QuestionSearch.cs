using System.Collections.Generic;
using System.Linq;
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
        private readonly ICseSearch _search;

        public QuestionSearch(ICseSearch search)
        {
            _search = search;
        }

        public async Task<IEnumerable<SearchResult>> SearchAsync(SearchQuery model, CancellationToken token)
        {
            var term = new List<string>();
            if (model.Query != null)
            {
                term.AddNotNull(string.Join(" ", model.Query));
            }

            if (term.Count == 0)
            {
                term.Add(QueryString);
            }

            var result = Enumerable.Range(model.Page * CseSearch.NumberOfPagesPerRequest, CseSearch.NumberOfPagesPerRequest).Select(s =>
            {
                var cseModel = new CseModel(term, model.Source, s, model.Sort, CustomApiKey.AskQuestion);
                return _search.DoSearchAsync(cseModel,
                    token);
            }).ToList();

            await Task.WhenAll(result).ConfigureAwait(false);
            return result.Where(s => s.Result != null).SelectMany(s => s.Result);
        }
    }
}