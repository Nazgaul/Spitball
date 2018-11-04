using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Sync;

namespace Cloudents.FunctionsV2.Sync
{
    public class DocumentDbToSearchSync : IDbToSearchSync
    {
        private readonly ISearchServiceWrite<Document> _questionServiceWrite;
        private readonly IQueryBus _bus;

        public DocumentDbToSearchSync(ISearchServiceWrite<Document> questionServiceWrite, IQueryBus bus)
        {
            _questionServiceWrite = questionServiceWrite;
            _bus = bus;
        }


        public Task CreateIndexAsync(CancellationToken token)
        {
            return _questionServiceWrite.CreateOrUpdateAsync(token);
        }

        public async Task<SyncResponse> DoSyncAsync(SyncAzureQuery query, CancellationToken token)
        {
            var (update, delete, version) =
                await _bus.QueryAsync<(IEnumerable<DocumentSearchDto> update, IEnumerable<string> delete, long version)>(query, token);
            var result = await _questionServiceWrite.UpdateDataAsync(update.Select(s => s.ToDocument()), delete, token);
            return new SyncResponse(version, result);
        }
    }
}