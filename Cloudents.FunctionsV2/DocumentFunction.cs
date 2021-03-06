using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Extension;
using Cloudents.FunctionsV2.FileProcessor;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NHibernate;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using NHibernate.Linq;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class DocumentFunction
    {
        [FunctionName("DocumentProcessFunction")]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public static async Task DocumentProcessFunctionAsync(
            [QueueTrigger("generate-search-preview")] string id,
            [Blob("spitball-files/files/{QueueTrigger}")] CloudBlobDirectory dir,
            [Blob("spitball-files/files/{QueueTrigger}/text.txt")] CloudBlockBlob blob,
            //[AzureSearchSync(DocumentSearchWrite.IndexName)]  IAsyncCollector<AzureSearchSyncOutput> indexInstance,
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

                //await indexInstance.AddAsync(new AzureSearchSyncOutput()
                //{
                //    Item = new Search.Entities.Document
                //    {
                //        Id = id,
                //        Content = text.Truncate(6000)
                //    },
                //    Insert = true
                //}, token);

            }
            catch (ObjectNotFoundException)
            {
                //await indexInstance.AddAsync(new AzureSearchSyncOutput()
                //{
                //    Item = new Search.Entities.Document
                //    {
                //        Id = id,
                //    },
                //    Insert = false
                //}, token);
            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                //Text blob was not found - somehow 
                //Do nothing
            }
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
        [SuppressMessage("ReSharper", "UnusedMember.Global")]

        public static async Task DeleteOldDocumentAsync([TimerTrigger("0 0 0 1 * *")] TimerInfo timer,
            [Blob("spitball-files/files")] CloudBlobDirectory directory,
            ILogger log,
            CancellationToken token)
        {
            BlobContinuationToken? blobToken = null;
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
        [SuppressMessage("ReSharper", "UnusedMember.Global")]

        public static async Task GeneratePreviewAsync(
            [QueueTrigger("generate-blob-preview-v2")] string id, int dequeueCount,
            [Blob("spitball-files/files/{QueueTrigger}")] CloudBlobDirectory directory,
            [Inject] IFileProcessorFactory factory,
            [Inject] IStatelessSession session,
            IBinder binder,
            ILogger log,
            CancellationToken token)
        {
            log.LogInformation($"receive preview for {id}");
            if (dequeueCount > 2)
            {
                log.LogInformation($"try to process more then 2 times");

                return;
            }
            var segment = await directory.ListBlobsSegmentedAsync(null);
            var originalBlob = (CloudBlockBlob)segment.Results.FirstOrDefault(f2 => f2.Uri.Segments.Last().StartsWith("file-"));
            if (originalBlob == null)
            {
                return;
            }

            var processor = factory.GetProcessor(originalBlob);
            if (processor is null)
            {
                log.LogError($"did not process id:{id}");
                return;
            }

            try
            {
                await processor.ProcessFileAsync(long.Parse(id), originalBlob, binder, log, token);
            }
            //catch (Cloudmersive.APIClient.NETCore.DocumentAndDataConvert.Client.ApiException ex)
            //{
            //    if (ex.Message.Contains("virus"))
            //    {
            //        await session.Query<Core.Entities.Document>().Where(w => w.Id == long.Parse(id))
            //            .UpdateBuilder()
            //            .Set(c => c.Status.State, x => ItemState.Deleted)
            //            .Set(c => c.Status.DeletedOn, x => DateTime.UtcNow)
            //            .Set(c => c.Status.FlagReason, x => "Virus")
            //            .UpdateAsync(token);
            //        foreach (var item in segment.Results.OfType<CloudBlockBlob>())
            //        {

            //            await item.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, AccessCondition.GenerateEmptyCondition(), new BlobRequestOptions(), new OperationContext(), token);
            //        }

            //    }

            //    originalBlob.Metadata["ErrorProcessCloudmersive"] = ex.Message;
            //    await originalBlob.SetMetadataAsync();
            //}
            catch (Exception ex)
            {
                originalBlob.Metadata["error"] = ex.Message;
                await originalBlob.SetMetadataAsync();
                throw;
            }

            log.LogInformation("C# Blob trigger function Processed");
        }


        [FunctionName("RemoveOldLocatorsVideo")]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]

        [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting", Justification = "Entry point")]
        public static async Task RemoveOldLocatorsVideoAsync(
            [TimerTrigger("0 0 1 * * *")] TimerInfo timer,
            [Inject] IVideoService videoService,
            CancellationToken token
            )
        {
            await videoService.RemoveUnusedStreamingLocatorAsync(token);
        }


        [SuppressMessage("ReSharper", "UnusedMember.Global")]

        [FunctionName("DocumentCalculateMd5")]
        public static async Task CalculateMd5Async(
          [TimerTrigger("0 0 1 * * *")] TimerInfo timer,
          [Inject] IStatelessSession session,
          IBinder binder,
          ILogger log,
          CancellationToken token
          )
        {
            var continue2 = true;
            while (continue2)
            {
                if (token.IsCancellationRequested)
                {
                    log.LogInformation("Finish due to cancellation token");
                    break;
                }
                var items = await session.Query<Core.Entities.Document>()
                    .Where(w => w.Status.State == ItemState.Ok && w.Md5 == null)
                    .OrderByDescending(x => x.Id)
                    .Select(s => s.Id).Take(100).ToListAsync(cancellationToken: token);
                continue2 = false;

                foreach (var id in items)
                {
                    if (token.IsCancellationRequested)
                    {
                        log.LogInformation("Finish due to cancellation token");
                        break;
                    }
                    continue2 = true;
                    log.LogInformation($"Processing {id}");
                    var blobDirectory = await binder.BindAsync<CloudBlobDirectory>(new BlobAttribute($"spitball-files/files/{id}"), token);
                    var blobs = await blobDirectory.ListBlobsSegmentedAsync(false, BlobListingDetails.Metadata, 100, null, null, null, token);
                    var blob = blobs.Results.OfType<CloudBlockBlob>().First(f =>
                        f.Name.StartsWith("file", StringComparison.OrdinalIgnoreCase));

                    var md5 = blob.Properties.ContentMD5;
                    if (string.IsNullOrEmpty(md5))
                    {
                        var length = blob.Properties.Length;
                        const long hugeFileThatWontFinishProcess = 13505445017;
                        if (length < hugeFileThatWontFinishProcess)
                        {
                            log.LogInformation("no md5 calculating");
                            await using var sr = await blob.OpenReadAsync();
                            md5 = CalculateMd5(sr);
                            log.LogInformation($"md5 is {md5}");

                            blob.Properties.ContentMD5 = md5;
                            foreach (var (key, _) in blob.Metadata.Where(w => string.IsNullOrEmpty(w.Value)))
                            {
                                blob.Metadata.Remove(key);
                            }

                            await blob.SetMetadataAsync();
                            await blob.SetPropertiesAsync();
                        }


                    }

                    md5 ??= blob.Properties.ETag;

                    if (md5 != null)
                    {
                        await session.Query<Core.Entities.Document>().Where(w => w.Id == id)
                            .UpdateBuilder()
                            .Set(c => c.Md5, x => md5)
                            .UpdateAsync(token);
                    }
                }
            }
        }

        private static string CalculateMd5(Stream stream)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(stream);
            var base64String = Convert.ToBase64String(hash);
            return base64String;
        }





    }
}
