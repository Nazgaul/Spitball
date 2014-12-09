using System;
using System.Collections.Generic;
using System.Threading;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Microsoft.WindowsAzure.Storage;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using System.Threading.Tasks;

namespace Zbang.Cloudents.OneTimeWorkerRole
{
    public class UpdateThumbnails : IUpdateThumbnails
    {
        // private IFileConvertFactory m_FileContertFactory;
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private readonly IZboxWriteService m_ZboxService;
        private readonly IZboxReadService m_ZboxReadService;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadServiceWorkerRole;

        public UpdateThumbnails(IFileProcessorFactory fileProcessorFactory,
            IZboxWriteService zboxService, IZboxReadService zboxReadService,
            IZboxReadServiceWorkerRole zboxReadServiceWorkerRole)
        {
            m_FileProcessorFactory = fileProcessorFactory;
            m_ZboxReadService = zboxReadService;
            m_ZboxService = zboxService;
            m_ZboxReadServiceWorkerRole = zboxReadServiceWorkerRole;
        }

        public void UpdateThumbnailPicture()
        {
            try
            {
                TraceLog.WriteInfo("Starting process of changing Pic");
                var cloudStorageAccount = CloudStorageAccount.Parse(
                    Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("StorageConnectionString"));


                var blobClient = cloudStorageAccount.CreateCloudBlobClient();

                // var thumbnailContainer = blobClient.GetContainerReference(BlobProvider.azureThumbnailContainer.ToLower());
                var fileContainer = blobClient.GetContainerReference(BlobProvider.AzureBlobContainer.ToLower());

                var blobs = new List<string>
                {
                     "f8a6d1b4-8625-4b9b-be69-78b0d13d93fc.h",
                };
                //var blobs = m_ZboxReadServiceWorkerRole.GetMissingThumbnailBlobs().Result;
                foreach (var blobname in blobs)
                {

                    var blob = fileContainer.GetBlockBlobReference(blobname);
                    try
                    {

                        TraceLog.WriteInfo("processing now " + blob.Uri);
                        UpdateFile2(blob.Uri);
                    }
                    catch (StorageException)
                    {
                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError("UpdateThumbnailPicture blob:" + blob.Uri, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("UpdateThumbnailPicture ", ex);
            }

            TraceLog.WriteInfo("End process of changing Pic");
        }

        private void UpdateFile2(Uri blobUri)
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            var processor = m_FileProcessorFactory.GetProcessor(blobUri);
            if (processor == null) return;

            //taken from : http://blogs.msdn.com/b/nikhil_agarwal/archive/2014/04/02/10511934.aspx
            var wait = new ManualResetEvent(false);

            var work = new Thread(async () =>
            {
                var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(TimeSpan.FromMinutes(3));
                //some long running method requiring synchronization
                var retVal = await processor.PreProcessFile(blobUri,tokenSource.Token);
                if (retVal == null)
                {
                    return;
                }
                var itemid = m_ZboxReadService.GetItemIdByBlobId(blobName);
                if (itemid == 0)
                {
                    throw new ArgumentException("cannot be 0", "itemid");
                }
                var command = new UpdateThumbnailCommand(itemid, retVal.ThumbnailName, retVal.BlobName, blobName,
                    retVal.FileTextContent);
                m_ZboxService.UpdateThumbnailPicture(command);
                wait.Set();
            });
            work.Start();
            Boolean signal = wait.WaitOne(TimeSpan.FromMinutes(3));
            if (!signal)
            {
                work.Abort();
                TraceLog.WriteError("blob url aborting process");


            }
            

        }

        //private void UpdateFile(Uri blobUri)
        //{
        //    //TEST
        //    var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
        //    var processor = m_FileProcessorFactory.GetProcessor(blobUri);
        //    if (processor == null) return;
        //    var tokenSource = new CancellationTokenSource();
        //    tokenSource.CancelAfter(TimeSpan.FromMinutes(2));
        //    CancellationToken token = tokenSource.Token;



        //    var t = Task.Factory.StartNew(
        //        () =>
        //            processor.PreProcessFile(blobUri, token), token);

        //    try
        //    {

        //        if (!t.Wait(600, token))
        //        {
        //            t.Dispose();
        //        }
        //    }
        //    catch (AggregateException e)
        //    {
        //        TraceLog.WriteError(e);
        //        return;
        //    }
        //    var retVal = t.Result.Result;
        //    if (retVal == null)
        //    {
        //        return;
        //    }
        //    var itemid = m_ZboxReadService.GetItemIdByBlobId(blobName);
        //    if (itemid == 0)
        //    {
        //        throw new ArgumentException("cannot be 0", "itemid");
        //    }
        //    var command = new UpdateThumbnailCommand(itemid, retVal.ThumbnailName, retVal.BlobName, blobName,
        //        retVal.FileTextContent);
        //    m_ZboxService.UpdateThumbnailPicture(command);

        //}



    }
    public interface IUpdateThumbnails
    {
        void UpdateThumbnailPicture();
    }
}
