using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;

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

        public async Task<long> DoSyncAsync(SyncAzureQuery query, CancellationToken token)
        {
            var (update, delete, version) =
                await _bus.QueryAsync(query, token);
            await _questionServiceWrite.UpdateDataAsync(update, delete.Select(s => s.ToString()), token);
            return version;
        }
    }
}