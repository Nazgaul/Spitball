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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.FunctionsV2.FileProcessor;
using Cloudents.Infrastructure.Video;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class DocumentFunction
    {
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
            try
            {
                var text = await blob.DownloadTextAsync();
                await blob.FetchAttributesAsync();
                var metadata = blob.Metadata;

                int? pageCount = null;
                if (metadata.TryGetValue("PageCount", out var pageCountStr) &&
                    int.TryParse(pageCountStr, out var pageCount2))
                {
                    pageCount = pageCount2;
                }


                var snippet = text.Truncate(200, true);
                var command = UpdateDocumentMetaCommand.Document(longId, pageCount, snippet);
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
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                //Text blob was not found - somehow 
                //Do nothing
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


        /// <summary>
        /// Delete old files that never commit to the database
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="directory"></param>
        /// <param name="log"></param>
        /// <param name="token"></param>
        /// <returns></returns>
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


        [FunctionName("BlobPreviewGenerator")]
        public static async Task GeneratePreviewAsync(
            [QueueTrigger("generate-blob-preview-v2")] string id,
            [Blob("spitball-files/files/{QueueTrigger}")]CloudBlobDirectory directory,
            [Inject] IFileProcessorFactory factory,
            ILogger log,
            CancellationToken token)
        {

            log.LogInformation($"receive preview for {id}");
            var segment = await directory.ListBlobsSegmentedAsync(null);
            var originalBlob = (CloudBlockBlob)segment.Results.FirstOrDefault(f2 => f2.Uri.Segments.Last().StartsWith("file-"));
            if (originalBlob == null)
            {
                return;
            }

            var processor = factory.GetProcessor(originalBlob);
            await processor.ProcessFileAsync(long.Parse(id), originalBlob, log, token);
            log.LogInformation("C# Blob trigger function Processed");
        }


        [FunctionName("media-service-event")]
        public static async Task Run(
            [QueueTrigger("media-service")] string message,
            [Inject] VideoProcessor videoProvider,
            IBinder binder,
            CancellationToken token)
        {

            dynamic json = JToken.Parse(message);
            foreach (var output in json.data.outputs)
            {
                string label = output.label;
                string assetName = output.assetName;
                var id = long.Parse(RegEx.NumberExtractor.Match(assetName).Value);
                if (label == MediaServices.JobLabelImage)
                {
                    await videoProvider.MoveImageAsync(id, binder, token);
                }

                if (label == MediaServices.JobLabelShortVideo)
                {
                    await videoProvider.CreateLocatorAsync(id, token);
                }

                if (label == MediaServices.JobLabelFullVideo)
                {
                    await videoProvider.UpdateDurationAsync(id, binder, token);
                }
            }
            
        }


    }
}
