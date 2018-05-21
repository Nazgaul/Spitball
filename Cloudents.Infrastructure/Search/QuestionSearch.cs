using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Search
{
    public class QuestionSearch : IQuestionSearch
    {
        private readonly ISearchIndexClient _client;

        public QuestionSearch(ISearchServiceClient client,string indexName)
        {
            //TODO: need to fix that before production
            _client = client.Indexes.GetClient(indexName);
        }

        public async Task<ResultWithFacetDto<Question>> SearchAsync(string term, [CanBeNull] IEnumerable<string> facet, CancellationToken token)
        {
            string filterStr = null;

            if (facet != null)
            {
                filterStr = string.Join(" or ", facet.Select(s =>
                    $"{nameof(Question.Subject)} eq '{s}'"));
            }

            var searchParameter = new SearchParameters
            {
                Facets = new[] { nameof(Question.Subject) },
                Filter = filterStr
            };


            var result = await
                _client.Documents.SearchAsync<Question>(term, searchParameter,
                    cancellationToken: token).ConfigureAwait(false);

            var retVal = new ResultWithFacetDto<Question>
            {
                Result = result.Results.Select(s => s.Document)
            };
            if (result.Facets.TryGetValue(nameof(Question.Subject), out var p))
            {
                retVal.Facet = p.Select(s => s.AsValueFacetResult<string>().Value);
            }
            return retVal;
        }
    }
}