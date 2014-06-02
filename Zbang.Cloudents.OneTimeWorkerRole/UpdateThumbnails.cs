using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Zbang.Zbox.Infrastructure.File;
using System.IO;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Azure.Blob;

namespace Zbang.Cloudents.OneTimeWorkerRole
{
    public class UpdateThumbnails : IUpdateThumbnails
    {
        private IThumbnailProvider m_ThumbnailProvider;
        private IBlobProvider m_BlobProvider;
        // private IFileConvertFactory m_FileContertFactory;
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private IZboxWriteService m_ZboxService;
        private IZboxReadService m_ZboxReadService;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadServiceWorkerRole;

        public UpdateThumbnails(IBlobProvider blobProvider, IThumbnailProvider thumbnailProvider, IFileProcessorFactory fileProcessorFactory,
            IZboxWriteService zboxService, IZboxReadService zboxReadService,
            IZboxReadServiceWorkerRole zboxReadServiceWorkerRole)
        {
            m_ThumbnailProvider = thumbnailProvider;
            m_BlobProvider = blobProvider;
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

//                var blobs = new List<string>() {
                    
////"989fa4b8-897a-47dc-8c8b-da1d84a3f1b0.pdf",
////"67fc54ff-db81-48a5-9a88-9b3efd5ea328.docx",
////"d5b2369a-8eb8-4f93-90f9-d4af08d7c89d.pdf",
////"df40b48c-b231-4da4-b26a-936f4ab15ab3.pdf",
////"9f7b0ee0-c5b1-4274-9df0-967019611cc0.doc",
////"65fc3bf8-211c-4e5f-ad26-c1fc3d5bb9d9.pdf",
//"7b4ec14e-c561-466c-8338-e4732c931bf7.pdf"
//                //                    "2d2446f6-c0dd-438f-9ec6-84fb78d2b4e1.htm",
                                    
//                };
                var blobs = m_ZboxReadServiceWorkerRole.GetMissingThumbnailBlobs().Result;
                foreach (var blobname in blobs)
                {

                    var blob = fileContainer.GetBlockBlobReference(blobname);
                    Guid fileName = Guid.Empty;
                    var blobName = blob.Uri.Segments[blob.Uri.Segments.Length - 1];
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
            if (processor != null)
            {
                var t =  processor.PreProcessFile(blobUri).Result;

                //t.Wait(TimeSpan.FromSeconds(30));
                var retVal = t;
                if (retVal == null)
                {
                    return;
                }
                var itemid = m_ZboxReadService.GetItemIdByBlobId(blobName);
                if (itemid == 0)
                {
                    throw new System.ArgumentException("cannot be 0", "itemid");
                }
                var command = new UpdateThumbnailCommand(itemid, retVal.ThumbnailName, retVal.BlobName, blobName, retVal.FileTextContent);
                m_ZboxService.UpdateThumbnailPicture(command);
            }
        }

        //private void GenerateCopyRights(string blobName)
        //{
        //    var stream = m_BlobProvider.DownloadFile(blobName);
        //    var retVal = processor.GenerateCopyRight(stream);
        //    var blob = m_BlobProvider.GetFile(blobName);
        //    using (var ms = new MemoryStream(retVal, false))
        //    {
        //        blob.UploadFromStream(ms);
        //    }
        //}


    }
    public interface IUpdateThumbnails
    {
        void UpdateThumbnailPicture();
    }
}
