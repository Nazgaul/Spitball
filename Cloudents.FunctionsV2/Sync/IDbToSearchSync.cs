﻿using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Query.Sync;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.Sync
{
    public interface IDbToSearchSync
    {
        //Task CreateIndexAsync(CancellationToken token);

        Task<SyncResponse> DoSyncAsync(SyncAzureQuery query, IBinder binder, CancellationToken token);
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