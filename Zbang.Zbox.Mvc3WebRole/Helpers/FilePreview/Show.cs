using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.ViewModel.DTOs;

namespace Zbang.Zbox.Mvc3WebRole.Helpers.FilePreview
{
    public class Show : PreviewBoxItem
    {
        IThumbnailProvider m_ThumbnailProvider;
        readonly ImageResizer m_ImageResizer;


        readonly Dictionary<Size, Size> m_PreviewDimenstion = new Dictionary<Size, Size> {
               {new Size(1024,768),new Size(929,929)},
               {new Size(1280,800),new Size(1184,1184)},
               {new Size(1366,768),new Size(1257,1257)},
               {new Size(1440,900),new Size(1331,1331)}
        };

        public Show(FileDto fileData, IBlobProvider blobProvider,
            IThumbnailProvider thumbnailProvider)
            : base(fileData, blobProvider)
        {
            m_ThumbnailProvider = thumbnailProvider;
            m_ImageResizer = new ImageResizer();
        }


        public override string PreviewItem(Size userScreenResolution)
        {
            return GenerateImagePreview(userScreenResolution);
        }

        private string GenerateImagePreview(Size userScreenResolution)
        {
            Size previewImageSize = GetPreviewImageSize(userScreenResolution);
            var cacheBlobName = generatePreviewFileName(FileData.BlobName, previewImageSize);

            if (BlobProvider.CheckIfFileExistsInCache(cacheBlobName))
            {
                return cacheBlobName;
            }
         
            var blob = BlobProvider.GetFile(FileData.BlobName);
            if (!blob.Exists())
            {
                throw new StorageClientException();
            }

            using (var ms = new MemoryStream())
            {
                blob.DownloadToStream(ms);
                var mResult = m_ImageResizer.ResizeImageAndSave(ms, previewImageSize.Height, previewImageSize.Width);
                BlobProvider.UploadFileToCache(cacheBlobName, mResult, blob.Properties.ContentType);

                return cacheBlobName;

            }
        }

        private Size GetPreviewImageSize(Size userScreenResolution)
        {
            var key = m_PreviewDimenstion.Keys.Aggregate((x, y) => Math.Abs(x.Width - userScreenResolution.Width) < Math.Abs(y.Width - userScreenResolution.Width) ? x : y);
            return m_PreviewDimenstion[key];
            //return previewDimenstion.Aggregate((x, y) => Math.Abs(x.Width - userScreenResolution.Width) < Math.Abs(y.Width - userScreenResolution.Width) ? x : y);
        }

        private string generatePreviewFileName(string blobFileName, Size sizeOfPreview)
        {
            return Path.GetFileNameWithoutExtension(blobFileName) + sizeOfPreview + Path.GetExtension(blobFileName);
        }


    }
}