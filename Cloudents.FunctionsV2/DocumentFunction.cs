using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
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
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            [Inject] ITextAnalysis textAnalysis,
            [Inject] ITextClassifier textClassifier,
            [Inject] ITextTranslator textTranslator,
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
            IEnumerable<string> tags = null;
            if (!metadata.ContainsKey("ProcessTags"))
            {
                tags = await GenerateTagsAsync(text, textAnalysis, textClassifier, textTranslator, token);
                metadata.Add("ProcessTags", bool.TrueString);
                await blob.SetMetadataAsync();
            }


            int? pageCount = null;
            if (metadata.TryGetValue("PageCount", out var pageCountStr) &&
                int.TryParse(pageCountStr, out var pageCount2))
            {
                pageCount = pageCount2;
            }

            try
            {
                var snippet = text.Truncate(200, true);
                var command = new UpdateDocumentMetaCommand(longId, pageCount, snippet, tags);
                await commandBus.DispatchAsync(command, token);

                await indexInstance.AddAsync(new AzureSearchSyncOutput()
                {
                    Item = new Document
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
                    Item = new Document
                    {
                        Id = id,
                    },
                    Insert = false
                }, token);
            }
        }

        private static async Task<IEnumerable<string>> GenerateTagsAsync(string text,
            ITextAnalysis textAnalysis,
            ITextClassifier textClassifier,
            ITextTranslator textTranslator,
            CancellationToken token)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            var englishCulture = new CultureInfo("en");

            var v = await textAnalysis.DetectLanguageAsync(text, token);
            if (!v.Equals(englishCulture))
            {
                text = await textTranslator.TranslateAsync(text, v.TwoLetterISOLanguageName, "en", token);
            }

            var keyPhrases = await textClassifier.KeyPhraseAsync(text, token);

            if (!v.Equals(englishCulture))
            {
                text = string.Join(" , ", keyPhrases);
                text = await textTranslator.TranslateAsync(text, "en", v.TwoLetterISOLanguageName.ToLowerInvariant(), token);

                return text.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
            }

            return keyPhrases.Select(s => s.Trim());
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

        //[FunctionName("FlagPreviewFailed")]
        //public static async Task FlagPreviewFailedAsync(
        //    [QueueTrigger("generate-blob-preview-poison",Connection = "TempConnectionDev")] string id,
        //    [Inject] ICommandBus bus,
        //    CancellationToken token
        //    )
        //{

        //    var command = new FlagDocumentCommand(null, long.Parse(id), "Preview failed");
        //    await bus.DispatchAsync(command, token);

        //}


        //[FunctionName("FlagPreviewFailed2")]
        //public static async Task FlagPreviewFailedAsync2(
        //    [TimerTrigger("0 0 0 1 * *", RunOnStartup = true)] TimerInfo timer,
        //    [Inject] ICommandBus bus,
        //    CancellationToken token
        //)
        //{
        //    try
        //    {
        //        var command = new FlagDocumentCommand(null, 187836, "Preview failed");
        //        await bus.DispatchAsync(command, token);
        //    }
        //    catch (NotFoundException)
        //    {

        //    }

        //}

    }
}
