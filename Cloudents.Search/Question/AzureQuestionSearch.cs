using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Search.Extensions;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Common;

namespace Cloudents.Search.Question
{
    public class AzureQuestionSearch : IQuestionsSearch
    {
        private const int PageSize = 20;
        private readonly ISearchIndexClient _client;

        public AzureQuestionSearch(ISearchService client)
        {
            _client = client.GetClient(QuestionSearchWrite.IndexName);
        }

        public async Task GetById(string id)
        {
            var t = await _client.Documents.GetAsync<Entities.Question>(id);

        }

        public async Task<(IEnumerable<long> result, IEnumerable<QuestionSubject> facetSubject, IEnumerable<QuestionFilter> facetFileter)>
            SearchAsync(QuestionsQuery query, CancellationToken token)
        {
            var filters = new List<string>
            {
                $"({nameof(Entities.Question.Country)} eq '{query.Country.ToUpperInvariant()}' or {nameof(Entities.Question.Language)} eq 'en')"
            };
            if (query.Source != null)
            {
                var filterStr = string.Join(" or ", query.Source.Select(s =>
                    $"{nameof(Entities.Question.Subject)} eq {(int)s}"));

                if (!string.IsNullOrEmpty(filterStr))
                {
                    filters.Add($"({filterStr})");
                }
            }

            if (query.Filters != null)
            {
                var filterStr = string.Join(" or ", query.Filters.Select(s =>
                     $"{nameof(Entities.Question.State)} eq {(int)s}"));
                if (!string.IsNullOrEmpty(filterStr))
                {
                    filters.Add($"({filterStr})");
                }
            }
            var searchParameter = new SearchParameters
            {
                Filter = string.Join(" and ", filters),
                Select = new[] { nameof(Entities.Question.Id) },
                Top = PageSize,
                Skip = query.Page * PageSize,
                ScoringProfile = QuestionSearchWrite.ScoringProfile,
                ScoringParameters = new[]
                             {
                    new ScoringParameter
                    (QuestionSearchWrite.TagsCountryParameter
                        , new[] {query.Country}),
                }

            };
            if (!string.IsNullOrEmpty(query.Term))
            {
                searchParameter.Facets = new[]
                {
                    nameof(Entities.Question.Subject),
                    nameof(Entities.Question.State)
                };
            }

            var result = await
                _client.Documents.SearchAsync<Entities.Question>(query.Term, searchParameter,
                    cancellationToken: token).ConfigureAwait(false);

            IEnumerable<QuestionSubject> facetSubject = null;
            IEnumerable<QuestionFilter> questionFilter = null;

            if (result.Facets != null)
            {
                if (result.Facets.TryGetValue(nameof(Entities.Question.Subject), out var p))

                {
                    facetSubject = p.AsEnumFacetResult<QuestionSubject>();
                }

                if (result.Facets.TryGetValue(nameof(Entities.Question.State), out var p2))
                {
                    questionFilter = p2.AsEnumFacetResult<QuestionFilter>();
                    //retVal.FacetState = p2.Select(s => (QuestionFilter)s.AsValueFacetResult<long>().Value);
                }
            }

            return (result.Results.Select(s => Convert.ToInt64(s.Document.Id)), facetSubject, questionFilter);
            //return result;
        }


    }
}