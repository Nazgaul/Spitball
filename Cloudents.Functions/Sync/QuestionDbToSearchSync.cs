using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Query.Sync;

namespace Cloudents.Functions.Sync
{
    public class QuestionDbToSearchSync : IDbToSearchSync
    {
        private readonly ISearchServiceWrite<Question> _questionServiceWrite;
        private readonly IQueryBus _bus;

        public QuestionDbToSearchSync(ISearchServiceWrite<Question> questionServiceWrite, IQueryBus bus)
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
                await _bus.QueryAsync<(IEnumerable<QuestionSearchDto> update, IEnumerable<long> delete, long version)>(query, token);
            var result = await _questionServiceWrite.UpdateDataAsync(update.Select(s => s.ToQuestion()), delete.Select(s => s.ToString()), token);
            return new SyncResponse(version, result);
        }
    }
}