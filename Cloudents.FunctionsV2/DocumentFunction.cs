using Cloudents.FunctionsV2.Binders;
using Cloudents.FunctionsV2.Sync;
using Cloudents.Search.Document;
using Cloudents.Search.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Extension;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class DocumentFunction
    {
        [FunctionName("BlobFunction")]
        public static async Task RunAsync(
            [BlobTrigger("spitball-files/files/{id}/text.txt")]string text, long id, IDictionary<string, string> metadata,
            [AzureSearchSync(DocumentSearchWrite.IndexName)]  IAsyncCollector<AzureSearchSyncOutput> indexInstance,
            [Inject] ICommandBus commandBus,
            CancellationToken token)
        {
            await SyncBlobWithSearch(text, id, metadata, indexInstance, commandBus, token);
        }


        [FunctionName("ReduBlobFunction")]
        public static async Task ReduBlobFunctionAsync(
            [QueueTrigger("generate-search-preview", Connection = "TempConnectionDev")] string id,
            [Blob("spitball-files/files/{QueueTrigger}", Connection = "TempConnectionDev")]CloudBlobDirectory dir,// IDictionary<string, string> metadata,
            [Blob("spitball-files/files/{QueueTrigger}/text.txt", Connection = "TempConnectionDev")]CloudBlockBlob blob,// IDictionary<string, string> metadata,
            [AzureSearchSync(DocumentSearchWrite.IndexName)]  IAsyncCollector<AzureSearchSyncOutput> indexInstance,
            [Inject] ICommandBus commandBus,

            CancellationToken token)
        {
            var x = await dir.ListBlobsSegmentedAsync(null);
            if (!x.Results.Any())
            {
                //There is no file - deleting it.
                var command = new DeleteDocumentCommand(Convert.ToInt64(id));
                await commandBus.DispatchAsync(command, token);
                return;
            }
            var text = await blob.DownloadTextAsync();
            await blob.FetchAttributesAsync();
            var metadata = blob.Metadata;
            await SyncBlobWithSearch(text, Convert.ToInt64(id), metadata, indexInstance, commandBus, token);
        }

        //[FunctionName("BlobFunctionTimer")]
        //public static async Task RunAsync2(
        //    [TimerTrigger("0 */20 * * * *", RunOnStartup = true)]TimerInfo timer,
        //    [Blob("spitball-files/files/1082/text.txt", Connection = "TempConnectionDev")] CloudBlockBlob blob,
        //    [Inject] ISearchServiceWrite<Document> searchInstance,
        //    [Inject] ICommandBus commandBus,
        //    [Inject] ITextAnalysis textAnalysis,
        //    ILogger log, CancellationToken token)
        //{
        //    var text = await blob.DownloadTextAsync();
        //    var metadata = blob.Metadata;
        //    await SyncBlobWithSearch(text, 1082, metadata, searchInstance, commandBus, textAnalysis, token);
        //}

        private static async Task SyncBlobWithSearch(string text, long id, IDictionary<string, string> metadata,
            IAsyncCollector<AzureSearchSyncOutput> searchInstance, ICommandBus commandBus, CancellationToken token)
        {
            int? pageCount = null;
            if (metadata.TryGetValue("PageCount", out var pageCountStr) &&
                int.TryParse(pageCountStr, out var pageCount2))
            {
                pageCount = pageCount2;
            }
            try
            {
                var snippet = text.Truncate(200, true);
                var command = new UpdateDocumentMetaCommand(id, pageCount, snippet);
                await commandBus.DispatchAsync(command, token);

                await searchInstance.AddAsync(new AzureSearchSyncOutput()
                {
                    Item = new Document
                    {
                        Id = id.ToString(),
                        Content = text.Truncate(6000)
                    },
                    Insert = true
                }, token);

            }
            catch (ObjectNotFoundException)
            {
                await searchInstance.AddAsync(new AzureSearchSyncOutput()
                {
                    Item = new Document
                    {
                        Id = id.ToString(),
                    },
                    Insert = false
                }, token);
            }
        }


        [FunctionName("DocumentSearchSync")]
        public static async Task RunQuestionSearchAsync([TimerTrigger("0 10,40 * * * *", RunOnStartup = true)]
            TimerInfo _,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            await SyncFunc.StartSearchSync(starter, log, SyncType.Document);
        }


        [FunctionName("DocumentDeleteOld")]
        public static async Task DeleteOldDocument([TimerTrigger("0 0 0 1 * *", RunOnStartup = true)] TimerInfo _,
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
                    if (blob is CloudBlobDirectory)
                    {
                        continue;
                    }

                    if (blob is CloudBlockBlob b)
                    {
                        if (b.Properties.Created > DateTime.UtcNow.AddDays(-7))
                        {
                            continue;
                        }

                        if (b.Uri.Segments.Length != 4)
                        {
                            continue;
                        }
                        log.LogInformation($"Delete {b.Uri}");
                        await b.DeleteAsync();
                    }

                    // await blob.DeleteAsync();
                }
            } while (blobToken != null);
            log.LogInformation("Finish delete items");
        }

    }
}
