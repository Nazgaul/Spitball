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

                //var blobs = new List<string>
                //{
                //     "ee7fe53c-9e75-4a65-8a2e-1f59130a0be7.mp4",
                //};
                var blobs = m_ZboxReadServiceWorkerRole.GetMissingThumbnailBlobs().Result;
                foreach (var blobname in blobs)
                {

                    var blob = fileContainer.GetBlockBlobReference(blobname);
                    try
                    {
                        TraceLog.WriteInfo("processing now " + blob.Uri);
                        UpdateFile(blob.Uri);
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

        private void UpdateFile(Uri blobUri)
        {
            //TEST
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            var processor = m_FileProcessorFactory.GetProcessor(blobUri);
            if (processor == null) return;
            var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(TimeSpan.FromMinutes(240));
            CancellationToken token = tokenSource.Token;

          

            using (var t = Task.Factory.StartNew(() => processor.PreProcessFile(blobUri), token))
            {
                t.Wait(token);
                var retVal = t.Result.Result;
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
            }
        }
      


    }
    public interface IUpdateThumbnails
    {
        void UpdateThumbnailPicture();
    }
}
