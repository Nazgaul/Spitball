using Cloudents.Core.Query;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Functions.Sync
{
    public interface IDbToSearchSync
    {
        Task CreateIndexAsync(CancellationToken token);

        Task<SyncResponse> DoSyncAsync(SyncAzureQuery query, CancellationToken token);
    }

    public class SyncResponse
    {
        public SyncResponse(long version, bool needContinue)
        {
            Version = version;
            NeedContinue = needContinue;
        }

        public long Version { get; set; }
        public bool NeedContinue { get; set; }
    }
}