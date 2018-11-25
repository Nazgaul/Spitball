﻿using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.WebJobs;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2.Binders
{
    public class AzureSearchSyncAsyncCollector : IAsyncCollector<AzureSearchSyncOutput>
    {
        private readonly ISearchIndexClient _indexClient;

        private readonly Collection<AzureSearchSyncOutput> _messages = new Collection<AzureSearchSyncOutput>();

        public AzureSearchSyncAsyncCollector(ISearchIndexClient indexClient)
        {
            this._indexClient = indexClient;
        }

        public Task AddAsync(AzureSearchSyncOutput item, CancellationToken cancellationToken = default(CancellationToken))
        {
            _messages.Add(item);
            return Task.CompletedTask;
            //IndexBatch.MergeOrUpload()
            //throw new global::System.NotImplementedException();
        }

        public async Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var uploadAction = _messages.Where(w => w.Insert).Select(s => IndexAction.MergeOrUpload(s.Item));
            var deleteAction = _messages.Where(w => !w.Insert).Select(s => IndexAction.Delete(s.Item));

            var batch = IndexBatch.New(uploadAction.Union(deleteAction));

            await _indexClient.Documents.IndexAsync(batch, cancellationToken: cancellationToken);
        }
    }
}