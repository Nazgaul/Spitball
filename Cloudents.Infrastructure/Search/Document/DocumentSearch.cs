using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Search.Document
{
    public class DocumentSearch : IDocumentSearch
    {
        private readonly AzureDocumentSearch _client;
        private readonly IQueryBus _queryBus;

        public DocumentSearch(AzureDocumentSearch client, IQueryBus queryBus)
        {
            _client = client;
            _queryBus = queryBus;
        }

        public Task<string> ItemContentAsync(long itemId, CancellationToken cancelToken)
        {
            return _client.ItemContentAsync(itemId, cancelToken);
        }

        public async Task<ResultWithFacetDto2<DocumentFeedDto>> SearchDocumentsAsync(DocumentQuery query,
            CancellationToken token)
        {
            var searchResult = await _client.SearchAsync(query, token);
            var ids = searchResult.Results.Select(s => long.Parse(s.Document.Id));
            var queryDb = new IdsQuery<long>(ids);
            var dbResult = await _queryBus.QueryAsync<IList<DocumentFeedDto>>(queryDb, token);
            var dic = dbResult.ToDictionary(x => x.Id);

            var retVal = new ResultWithFacetDto2<DocumentFeedDto>();

            foreach (var resultResult in searchResult.Results)
            {
                if (dic.TryGetValue(long.Parse(resultResult.Document.Id), out var p))
                {
                    p.Snippet = p.Snippet;
                    retVal.Result.Add(p);
                }
                
            }

            return retVal;
            //var retVal = new QuestionWithFacetDto { Result = dbResult };
        }

    }
}
