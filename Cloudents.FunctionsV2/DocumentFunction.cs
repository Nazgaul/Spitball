using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Extension;
using Cloudents.FunctionsV2.Binders;
using Cloudents.FunctionsV2.Sync;
using Cloudents.Search.Document;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NHibernate;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class DocumentFunction
    {
        [FunctionName("BlobFunction")]
        public static async Task RunAsync(
            [BlobTrigger("spitball-files/files/{id}/text.txt")]string text, long id,
            [Queue("generate-search-preview")] IAsyncCollector<string> collector,
            [AzureSearchSync(DocumentSearchWrite.IndexName)]  IAsyncCollector<AzureSearchSyncOutput> indexInstance,
            CancellationToken token)
        {
            await collector.AddAsync(id.ToString(), token);
            //await SyncBlobWithSearch(text, id, metadata, indexInstance, commandBus, token);
        }



        [FunctionName("DocumentProcessFunction")]
        public static async Task DocumentProcessFunctionAsync(
            [QueueTrigger("generate-search-preview")] string id,
            [Blob("spitball-files/files/{QueueTrigger}")]CloudBlobDirectory dir,
            [Blob("spitball-files/files/{QueueTrigger}/text.txt")]CloudBlockBlob blob,
            [AzureSearchSync(DocumentSearchWrite.IndexName)]  IAsyncCollector<AzureSearchSyncOutput> indexInstance,
            [Inject] ICommandBus commandBus,
            ILogger log,
            CancellationToken token)
        {
            log.LogInformation($"Processing {id}");
            var x = await dir.ListBlobsSegmentedAsync(null);

            var longId = Convert.ToInt64(id);
            if (!x.Results.Any())
            {
                //There is no file - deleting it.
                var command = new DeleteDocumentCommand(longId);
                await commandBus.DispatchAsync(command, token);
                return;
            }
            var text = await blob.DownloadTextAsync();
            await blob.FetchAttributesAsync();
            var metadata = blob.Metadata;

            int? pageCount = null;
            if (metadata.TryGetValue("PageCount", out var pageCountStr) &&
                int.TryParse(pageCountStr, out var pageCount2))
            {
                pageCount = pageCount2;
            }

            try
            {
                var snippet = text.Truncate(200, true);
                var command = new UpdateDocumentMetaCommand(longId, pageCount, snippet);
                await commandBus.DispatchAsync(command, token);

                await indexInstance.AddAsync(new AzureSearchSyncOutput()
                {
                    Item = new Search.Entities.Document
                    {
                        Id = id,
                        Content = text.Truncate(6000)
                    },
                    Insert = true
                }, token);

            }
            catch (ObjectNotFoundException)
            {
                await indexInstance.AddAsync(new AzureSearchSyncOutput()
                {
                    Item = new Search.Entities.Document
                    {
                        Id = id,
                    },
                    Insert = false
                }, token);
            }
        }

        

        [FunctionName("DocumentSearchSync")]
        public static async Task RunQuestionSearchAsync([TimerTrigger("0 10,40 * * * *")]
            TimerInfo timer,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            await SyncFunc.StartSearchSync(starter, log, SyncType.Document);
        }


        [FunctionName("DocumentDeleteOld")]
        [UsedImplicitly]
        public static async Task DeleteOldDocument([TimerTrigger("0 0 0 1 * *")] TimerInfo timer,
            [Blob("spitball-files/files")]CloudBlobDirectory directory,
            ILogger log,
            CancellationToken token)
        {
            BlobContinuationToken blobToken = null;
            do
            {

                var files = await directory.ListBlobsSegmentedAsync(false, BlobListingDetails.None, null, blobToken,
                    new BlobRequestOptions(),
                    new OperationContext(), token);
                log.LogInformation("Going to delete items");
                blobToken = files.ContinuationToken;
                foreach (var blob in files.Results)
                {
                    switch (blob)
                    {
                        case CloudBlobDirectory _:
                        case CloudBlockBlob b when b.Properties.Created > DateTime.UtcNow.AddDays(-7):
                            continue;
                        case CloudBlockBlob b when b.Uri.Segments.Length != 4:
                            continue;
                        case CloudBlockBlob b:
                            log.LogInformation($"Delete {b.Uri}");
                            await b.DeleteAsync();
                            break;
                    }

                    // await blob.DeleteAsync();
                }
            } while (blobToken != null);
            log.LogInformation("Finish delete items");
        }
    }
}
