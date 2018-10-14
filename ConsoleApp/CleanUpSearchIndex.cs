using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Infrastructure.Data;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class CleanUpSearchIndex
    {
        private readonly ISearchServiceWrite<Document> _searchServiceWrite;
        private readonly IDocumentSearch _documentSearch;
        private readonly DapperRepository _repository;


        public CleanUpSearchIndex(ISearchServiceWrite<Document> searchServiceWrite, IDocumentSearch documentSearch, DapperRepository repository)
        {
            _searchServiceWrite = searchServiceWrite;
            _documentSearch = documentSearch;
            _repository = repository;
        }

        public async Task DoCleanUp()
        {
            var page = 0;
            var docs = await _documentSearch.SearchDocumentsAsync(SearchQuery.Document(null, null, null, null, page), default);

            var searchIds = docs.Result.Select(s => long.Parse(s.Id));
            IEnumerable<long> dbItemIds = Enumerable.Empty<long>();

            using (var conn = _repository.OpenConnection())
            {
                dbItemIds = await conn.QueryAsync<long>("select ItemId from zbox.item where itemid in @itemids and isdeleted = 0",
                   new { itemids = searchIds });
            }

            var result = searchIds.Except(dbItemIds).ToList();

            if (result.Count > 0)
            {
                await _searchServiceWrite.DeleteDataAsync(result.Select(s => s.ToString()), default);
            }

        }
    }
}