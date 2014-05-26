using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Zbang.Zbox.Infrastructure.File;
using System.IO;
using System.Collections.Generic;

namespace Zbang.Zbox.WorkerRole.OneTimeUpdates
{
    public class UpdateThumbnails : IUpdateThumbnails
    {
        private IThumbnailProvider m_ThumbnailProvider;
        private IBlobProvider m_BlobProvider;
        // private IFileConvertFactory m_FileContertFactory;
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private IZboxWriteService m_ZboxService;
        private IZboxReadService m_ZboxReadService;

        public UpdateThumbnails(IBlobProvider blobProvider, IThumbnailProvider thumbnailProvider, IFileProcessorFactory fileProcessorFactory,
            IZboxWriteService zboxService, IZboxReadService zboxReadService)
        {
            m_ThumbnailProvider = thumbnailProvider;
            m_BlobProvider = blobProvider;
            m_FileProcessorFactory = fileProcessorFactory;
            // m_FileContertFactory = fileConvertFacotry;
            m_ZboxReadService = zboxReadService;
            m_ZboxService = zboxService;
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


                //var blobNames = new List<string>() {
                //    "70e2ab7d-76d8-4a3a-883a-c960c78762ad.pdf",
                //    "53eb3735-8ca0-4cc3-a799-bb1bf78ca91d.docx",
                //    "591d37d1-3c41-4f14-b612-61fca302a2ec.pptx",
                //    "3f7abc3b-7fe6-47aa-86ec-9a1ed7b40459.doc",
                //    "a09eaeba-6ab5-4e04-9846-85369f080ccd.xls",
                //    "98576ac7-3c15-40f5-93a8-b232a7c25659.xls",
                //    "b0f17be0-400e-4527-99e5-d7638317f605.tiff",
                //    "4f07a612-2277-4140-90b5-4b34bbc5a65b.ppt",
                //    "a5e8364d-eaa3-4328-b812-e4630c4ae01c.mov",
                //    "6021e84d-822f-4c8a-85f7-baa5c69cfc6c.ppt",
                //    "f00ef42c-f8c5-4888-888f-d0a61f1d1263.pdf"
                //};
                foreach (var blobname in fileContainer.ListBlobs(null, true, BlobListingDetails.All))
                {

                    //var blob = fileContainer.GetBlockBlobReference();
                    // Guid fileName = Guid.Empty;
                    //var blobName = blob.Uri.Segments[blob.Uri.Segments.Length - 1];
                    try
                    {
                        //  UpdateThumbnail(blobName);
                        UpdateFile(blobname.Uri);
                    }
                    catch (StorageException)
                    {
                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError("UpdateThumbnailPicture blob:" + blobname.Uri, ex);
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
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            var processor = m_FileProcessorFactory.GetProcessor(blobUri);
            if (processor != null)
            {
                var t = processor.PreProcessFile(blobUri);
                t.Wait();
                var retVal = t.Result;

                var itemid = m_ZboxReadService.GetItemIdByBlobId(blobName);
                if (itemid == 0)
                {
                    throw new System.ArgumentException("cannot be 0", "itemid");
                }



                var command = new UpdateThumbnailCommand(itemid, retVal.ThumbnailName, retVal.BlobName, blobName, retVal.FileTextContent);
                m_ZboxService.UpdateThumbnailPicture(command);
                //GenerateCopyRights(blobName, processor);
                //GenerateFileCache(blobName, processor);
            }
        }

        //private void GenerateCopyRights(string blobName, IFileProcessor processor)
        //{
        //    var stream = m_BlobProvider.DownloadFile(blobName);
        //    var retVal = processor.GenerateCopyRight(stream);
        //    var blob = m_BlobProvider.GetFile(blobName);
        //    using (var ms = new MemoryStream(retVal, false))
        //    {
        //        blob.UploadFromStream(ms);
        //    }
        //}
        //private void GenerateFileCache(string blobName, IFileProcessor processor)
        //{
        //    var blobCacheName = string.Format("{0}V3.pdf", System.IO.Path.GetFileNameWithoutExtension(blobName));
        //    var stream = m_BlobProvider.DownloadFile(blobName);
        //    var retVal = processor.ConvertFileToWebSitePreview(stream, 0, 0);
        //    Compress compressor = new Compress();
        //    var byteArray = compressor.CompressToGzip(retVal);

        //    m_BlobProvider.UploadFileToCache(blobCacheName, byteArray, "application/pdf", true);

        //}

        //private void UpdateThumbnail(string blobName)
        //{
        //    var itemid = m_ZboxReadService.GetItemIdByBlobId(blobName);
        //    if (itemid == 0)
        //    {
        //        throw new System.ArgumentException("cannot be 0", "itemid");
        //    }
        //    var thumbnail = m_ThumbnailProvider.GenerateThumbnail(blobName);
        //    var command = new UpdateThumbnailCommand(itemid, thumbnail, blobName, blobName);
        //    m_ZboxService.UpdateThumbnailPicture(command);
        //}
    }
    public interface IUpdateThumbnails
    {
        void UpdateThumbnailPicture();
    }
}
