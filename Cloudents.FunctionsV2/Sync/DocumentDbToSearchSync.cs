﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Sync;
using Cloudents.FunctionsV2.Binders;
using Cloudents.Infrastructure.Write;
using Cloudents.Search.Document;
using Cloudents.Search.Entities;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.Sync
{
    public class DocumentDbToSearchSync : IDbToSearchSync
    {
       // private readonly ISearchServiceWrite<Document> _questionServiceWrite;
        private readonly IQueryBus _bus;

        public DocumentDbToSearchSync( IQueryBus bus)
        {
            _bus = bus;
        }


        //public Task CreateIndexAsync(CancellationToken token)
        //{
        //    return _questionServiceWrite.CreateOrUpdateAsync(token);
        //}

        public async Task<SyncResponse> DoSyncAsync(SyncAzureQuery query,IBinder binder, CancellationToken token)
        {
            var (update, delete, version) =
                await _bus.QueryAsync<(IEnumerable<DocumentSearchDto> update, IEnumerable<string> delete, long version)>(query, token);

            var syncService = await binder.BindAsync<
                IAsyncCollector<AzureSearchSyncOutput>>(
                new AzureSearchSyncAttribute(DocumentSearchWrite.IndexName), token);

            var needContinue = false;
            foreach (var questionUpdate in update)
            {

                needContinue = true;
                await syncService.AddAsync(new AzureSearchSyncOutput()
                {
                    Item = Document.FromDto(questionUpdate),
                    Insert = true

                }, token);
            }

            foreach (var questionDelete in delete)
            {
                needContinue = true;
                await syncService.AddAsync(new AzureSearchSyncOutput()
                {
                    Item = new Question()
                    {
                        Id = questionDelete
                    },
                    Insert = false

                }, token);
            }

            await syncService.FlushAsync(token);

            // var result = await _questionServiceWrite.UpdateDataAsync(update.Select(s => s.ToDocument()), delete, token);
            return new SyncResponse(version, needContinue);
        }
    }
}