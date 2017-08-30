using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Microsoft.WindowsAzure.Storage;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using System.Threading.Tasks;
using System.Runtime;

namespace Zbang.Cloudents.OneTimeWorkerRole
{
    public class UpdateThumbnails : IUpdateThumbnails
    {
        // private IFileConvertFactory m_FileContertFactory;
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private readonly IZboxWorkerRoleService m_ZboxService;
        private readonly IZboxReadService m_ZboxReadService;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadServiceWorkerRole;

        public UpdateThumbnails(IFileProcessorFactory fileProcessorFactory,
            IZboxWorkerRoleService zboxService, IZboxReadService zboxReadService,
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
                TraceLog.WriteWarning("Starting process of changing Pic");
                var cloudStorageAccount = CloudStorageAccount.Parse(
                    Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("StorageConnectionString"));


                var blobClient = cloudStorageAccount.CreateCloudBlobClient();
                //var container = blobClient.GetContainerReference("deployn");
                //var blobId = container.GetBlockBlobReference("id.txt");
                //var txt = blobId.DownloadText();
                var id = 0;//Convert.ToInt64(txt);
                // var thumbnailContainer = blobClient.GetContainerReference(BlobProvider.azureThumbnailContainer.ToLower());
                var fileContainer = blobClient.GetContainerReference(BlobProvider.AzureBlobContainer.ToLower());
                var previewContainer = blobClient.GetContainerReference(BlobProvider.AzurePreviewContainer.ToLower());

                //var blobs = new List<string>
                //{
                //     "f8a6d1b4-8625-4b9b-be69-78b0d13d93fc.h",
                //};
                //int index = 0;
                bool cont = true;
                while (cont)
                {
                    //blobId.UploadText(id.ToString(CultureInfo.InvariantCulture));
                    TraceLog.WriteWarning("processing now index starting from  " + id);
                    var items = m_ZboxReadServiceWorkerRole.GetMissingThumbnailBlobsAsync(0, id).Result.ToList();
                    if (!items.Any())
                    {
                        cont = false;
                    }
                    //index++;
                    foreach (var blobname in items)
                    {
                        if (blobname.itemid < id)
                        {
                            continue;
                        }
                        var blob = fileContainer.GetBlockBlobReference(blobname.blobname);

                        Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob blobInPreview = previewContainer.GetBlockBlobReference(blobname.blobname + ".jpg");
                        if (blobInPreview.Exists())
                        {
                            continue;
                        }
                        try
                        {

                            TraceLog.WriteInfo("processing now " + blob.Uri + " id: " + blobname.itemid);
                            UpdateFile2(blob.Uri, blobname.itemid);
                        }
                        catch (StorageException)
                        {
                        }
                        catch (Exception ex)
                        {
                            TraceLog.WriteError("UpdateThumbnailPicture blob:" + blob.Uri, ex);
                        }
                        id = blobname.itemid;
                    }
                    
                    //TraceLog.WriteInfo("collecting gc");
                    //GC.Collect();
                    //TraceLog.WriteInfo("end collecting gc");

                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("UpdateThumbnailPicture ", ex);
            }

            TraceLog.WriteWarning("End process of changing Pic");
        }
        private readonly TimeSpan timeToWaite = TimeSpan.FromMinutes(3);
        private void UpdateFile2(Uri blobUri, long itemId)
        {

            
            var processor = m_FileProcessorFactory.GetProcessor(blobUri);
            if (processor == null) return;
            if (processor is VideoProcessor ||
                processor is AudioProcessor ||
                processor is TextProcessor
                //processor is ImageProcessor ||
                //processor is TiffProcessor
                )
            {
                return;
            }


            //taken from : http://blogs.msdn.com/b/nikhil_agarwal/archive/2014/04/02/10511934.aspx
            var wait = new ManualResetEvent(false);

            var work = new Thread(async () =>
            {
                try
                {
                    var tokenSource = new CancellationTokenSource();
                    tokenSource.CancelAfter(timeToWaite);
                    //some long running method requiring synchronization
                    var retVal = await processor.PreProcessFileAsync(blobUri, tokenSource.Token);
                    if (retVal == null)
                    {
                        return;
                    }


                    //var command = new UpdateThumbnailCommand(itemId, retVal.ThumbnailName, retVal.BlobName, blobName,
                    //    retVal.FileTextContent);
                    //m_ZboxService.UpdateThumbnailPicture(command);
                    wait.Set();
                }
                catch (Exception ex)
                {

                    TraceLog.WriteError("on itemid: " + itemId, ex);
                }
            });
            work.Start();
            Boolean signal = wait.WaitOne(timeToWaite);
            if (!signal)
            {
                work.Abort();
                TraceLog.WriteError("blob url aborting process. itemid: " + itemId);
            }
        }
    }
    public interface IUpdateThumbnails
    {
        void UpdateThumbnailPicture();
    }
}
