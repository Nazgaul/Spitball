using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.StorageClient;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.ViewModel.DTOs;

namespace Zbang.Zbox.Mvc3WebRole.Helpers
{
    public class ImagePreview //: PreviewBoxItem
    {
        readonly IThumbnailProvider m_ThumbnailProvider;
        readonly IBlobProvider m_BlobProvider;
        readonly ImageResizer m_ImageResizer;


        readonly Dictionary<ZboxSize, ZboxSize> m_PreviewDimenstion = new Dictionary<ZboxSize, ZboxSize> {
            {new ZboxSize(1920,1080),new ZboxSize(1728,972)},
            {new ZboxSize(1440,900),new ZboxSize(1296,810)},
               {new ZboxSize(1366,768),new ZboxSize(1230,692)},
               {new ZboxSize(1280,1024),new ZboxSize(1152,922)},
               {new ZboxSize(1280,800),new ZboxSize(1152,720)},
               {new ZboxSize(1024,768),new ZboxSize(922,692)}
               
               
        };

        public ImagePreview(IBlobProvider blobProvider,
            IThumbnailProvider thumbnailProvider)
        {
            m_ThumbnailProvider = thumbnailProvider;
            m_ImageResizer = new ImageResizer();
            m_BlobProvider = blobProvider;
        }

        public string GenerateImagePreview(string blobName, ZboxSize screenWidthSize)
        {
            ZboxSize previewImageSize = GetPreviewImageSize(screenWidthSize);
            var cacheBlobName = generatePreviewFileName(blobName, previewImageSize);

            if (m_BlobProvider.CheckIfFileExistsInCache(cacheBlobName))
            {
                return cacheBlobName;
            }

            var blob = m_BlobProvider.GetFile(blobName);
            if (!blob.Exists())
            {
                throw new StorageClientException();
            }

            using (var ms = new MemoryStream())
            {
                blob.DownloadToStream(ms);
                MemoryStream mResult = m_ImageResizer.ResizeImageAndSave(ms, previewImageSize.Height, previewImageSize.Width);
                m_BlobProvider.UploadFileToCache(cacheBlobName, mResult, blob.Properties.ContentType);

                return cacheBlobName;
            }
        }

        private ZboxSize GetPreviewImageSize(ZboxSize userScreenWidth)
        {
            if (userScreenWidth== null)
            {
                return new ZboxSize(585, 585);
            }
            var key = m_PreviewDimenstion.Keys.FirstOrDefault(f => f.Width <= userScreenWidth.Width && f.Height <= userScreenWidth.Height);
            //var key = m_PreviewDimenstion.Keys.Aggregate((x, y) => Math.Abs(x - userScreenWidth.Value.Width)
            //    < Math.Abs(y - userScreenWidth.Value.Width) ? x : y);

            if (key == null)
            {
                return new ZboxSize(585, 585);
            }
            return m_PreviewDimenstion[key];
        }

        private string generatePreviewFileName(string blobFileName, ZboxSize sizeOfPreview)
        {
            return Path.GetFileNameWithoutExtension(blobFileName) + sizeOfPreview + Path.GetExtension(blobFileName);
        }


    }

    public class ZboxSize
    {
        public ZboxSize(int? width, int? height)
        {
            Height = height.Value;
            Width = width.Value;
        }
        public int Height { get; set; }
        public int Width { get; set; }


        public static ZboxSize GenerateSize(int? width, int? height)
        {
            if (!width.HasValue || !height.HasValue)
            {
                return null;
            }
            else
            {
                return new ZboxSize(width, height);
            }
        }
        public override string ToString()
        {
            return Width + "*" + Height;
        }
    }
}