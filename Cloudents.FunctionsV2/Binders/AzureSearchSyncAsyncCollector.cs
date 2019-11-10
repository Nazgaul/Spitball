using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            _indexClient = indexClient;
        }

        public Task AddAsync(AzureSearchSyncOutput item, CancellationToken cancellationToken = default(CancellationToken))
        {
            _messages.Add(item);
            return Task.CompletedTask;
        }

        public async Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (_messages.Count == 0)
                {
                    return;
                }

                var uploadAction = _messages.Where(w => w.Insert).Select(s => IndexAction.MergeOrUpload(s.Item));
                var deleteAction = _messages.Where(w => !w.Insert).Select(s => IndexAction.Delete(s.Item));

                var batch = IndexBatch.New(uploadAction.Union(deleteAction));

                var result = await _indexClient.Documents.IndexAsync(batch, cancellationToken: cancellationToken);
                _messages.Clear();
#if DEBUG
                foreach (var errorResult in result.Results.Where(w => !w.Succeeded))
                {
                    Debug.WriteLine(
                        $"Failed to process id {errorResult.Key} error {errorResult.ErrorMessage} on index {_indexClient.IndexName} ");
                }

#endif
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }
    }
}