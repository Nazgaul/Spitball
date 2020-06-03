﻿using Cloudents.FunctionsV2.Binders;
using Cloudents.Query;
using Cloudents.Query.Sync;
using Cloudents.Search.Document;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2.Sync
{
    public class DocumentDbToSearchSync : IDbToSearchSync
    {
        // private readonly ISearchServiceWrite<Document> _questionServiceWrite;
        private readonly IQueryBus _bus;

        public DocumentDbToSearchSync(IQueryBus bus)
        {
            _bus = bus;
        }


        //public Task CreateIndexAsync(CancellationToken token)
        //{
        //    return _questionServiceWrite.CreateOrUpdateAsync(token);
        //}

        public async Task<SyncResponse> DoSyncAsync(SyncAzureQuery query, IBinder binder, CancellationToken token)
        {
            var (update, delete, version) =
                await _bus.QueryAsync(query, token);

            var syncService = await binder.BindAsync<
                IAsyncCollector<AzureSearchSyncOutput>>(
                new AzureSearchSyncAttribute(DocumentSearchWrite.IndexName), token);

            var needContinue = false;
            foreach (var document in update)
            {

                needContinue = true;
                await syncService.AddAsync(new AzureSearchSyncOutput
                {
                    Item = Search.Entities.Document.FromDto(document),
                    Insert = true

                }, token);
            }

            foreach (var document in delete)
            {
                needContinue = true;
                await syncService.AddAsync(new AzureSearchSyncOutput
                {
                    Item = new Search.Entities.Document()
                    {
                        Id = document
                    },
                    Insert = false

                }, token);
            }

            await syncService.FlushAsync(token);
            return new SyncResponse(version, needContinue);
        }
    }
}