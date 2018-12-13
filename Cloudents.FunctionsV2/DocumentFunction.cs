using System;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.FunctionsV2.Sync;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;
using NHibernate;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.FunctionsV2.Binders;
using Cloudents.Search.Document;
using Cloudents.Search.Entities;
using Cloudents.Search.Question;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class DocumentFunction
    {
        [FunctionName("BlobFunction")]
        public static async Task RunAsync(
            [BlobTrigger("spitball-files/files/{id}/text.txt")]string text, long id, IDictionary<string, string> metadata,
            //[Inject] ISearchServiceWrite<Document> searchInstance,
            [AzureSearchSync(DocumentSearchWrite.IndexName)]  IAsyncCollector<AzureSearchSyncOutput> indexInstance,
            [Inject] ICommandBus commandBus,
            [Inject] ITextAnalysis textAnalysis,

            CancellationToken token)
        {
            await SyncBlobWithSearch(text, id, metadata, indexInstance, commandBus, textAnalysis, token);
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
            IAsyncCollector<AzureSearchSyncOutput> searchInstance, ICommandBus commandBus, ITextAnalysis textAnalysis, CancellationToken token)
        {
            var lang = await textAnalysis.DetectLanguageAsync(text.Truncate(5000), token);
            int? pageCount = null;
            if (metadata.TryGetValue("PageCount", out var pageCountStr) &&
                int.TryParse(pageCountStr, out var pageCount2))
            {
                pageCount = pageCount2;
            }

            try
            {
                var command = new UpdateDocumentMetaCommand(id, lang, pageCount);
                await commandBus.DispatchAsync(command, token);



                await searchInstance.AddAsync(new AzureSearchSyncOutput()
                {
                    Item = new Document
                    {
                        Id = id.ToString(),
                        Content = text.Truncate(6000),
                        Language = lang.TwoLetterISOLanguageName,
                        MetaContent = text.Truncate(200, true)
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
            TimerInfo myTimer,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            await SyncFunc.StartSearchSync(starter, log, SyncType.Document);
        }


        [FunctionName("DocumentDeleteOld")]
        public static async Task DeleteOldDocument([TimerTrigger("0 0 0 1 * *", RunOnStartup = true)] TimerInfo myTimer,
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
