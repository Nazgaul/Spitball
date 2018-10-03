using Cloudents.Core.Query;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Functions.Sync
{
    public interface IDbToSearchSync
    {
        Task CreateIndexAsync(CancellationToken token);

        Task<long> DoSyncAsync(SyncAzureQuery query, CancellationToken token);
    }
}