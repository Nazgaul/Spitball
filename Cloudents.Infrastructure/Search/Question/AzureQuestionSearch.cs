using Cloudents.Core.Query;
using Cloudents.Infrastructure.Write;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Search.Question
{
    public class AzureQuestionSearch //: IQuestionSearch
    {
        private readonly ISearchIndexClient _client;

        public AzureQuestionSearch(ISearchService client)
        {
            _client = client.GetClient(QuestionSearchWrite.IndexName);
        }

        public async Task GetById(string id)
        {
            var t = await _client.Documents.GetAsync<Core.Entities.Search.Question>(id);
            
        }

        public async Task<DocumentSearchResult<Core.Entities.Search.Question>> SearchAsync(QuestionsQuery query, CancellationToken token)
        {
            var filters = new List<string>
            {
                $"({nameof(Core.Entities.Search.Question.Country)} eq '{query.Country.ToUpperInvariant()}' or {nameof(Core.Entities.Search.Question.Language)} eq 'en')"
            };
            if (query.Source != null)
            {
                var filterStr = string.Join(" or ", query.Source.Select(s =>
                    $"{nameof(Core.Entities.Search.Question.Subject)} eq {(int)s}"));

                filters.Add($"({filterStr})");
            }

            if (query.Filters != null)
            {
                var filterStr = string.Join(" or ", query.Filters.Select(s =>
                     $"{nameof(Core.Entities.Search.Question.State)} eq {(int)s}"));
                filters.Add($"({filterStr})");
            }
            var searchParameter = new SearchParameters
            {
                Filter = string.Join(" and ", filters),
                Select = new [] {nameof(Core.Entities.Search.Question.Id)},
                Top = 50,
                Skip = query.Page * 50,
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
                    nameof(Core.Entities.Search.Question.Subject),
                    nameof(Core.Entities.Search.Question.State)
                };
            }

            var result = await
                _client.Documents.SearchAsync<Core.Entities.Search.Question>(query.Term, searchParameter,
                    cancellationToken: token).ConfigureAwait(false);

            return result;
        }
    }
}