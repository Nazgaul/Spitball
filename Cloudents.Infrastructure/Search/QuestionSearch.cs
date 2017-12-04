﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;

namespace Cloudents.Infrastructure.Search
{
    public class QuestionSearch : IQuestionSearch
    {
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
                term.Add(string.Join(" ", model.Query));
            }


            var result = Enumerable.Range(model.Page * 3, 3).Select(s =>
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