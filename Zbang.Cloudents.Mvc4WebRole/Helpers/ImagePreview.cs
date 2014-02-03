using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class ImagePreview //: PreviewBoxItem
    {
        //private readonly IThumbnailProvider m_ThumbnailProvider;
        private readonly IBlobProvider m_BlobProvider;
        private readonly IFileProcessorFactory m_FileProcessorFactory;

        const int SubstractWidth = 0;
        const int SubstractHeight = 0;


        readonly Dictionary<ZboxSize, ZboxSize> m_PreviewDimenstion = new Dictionary<ZboxSize, ZboxSize> {
            {new ZboxSize(1920,1080),new ZboxSize(1920-SubstractWidth,1080-SubstractHeight)},
            {new ZboxSize(1440,900),new ZboxSize(1440-SubstractWidth,900-SubstractHeight)},
               {new ZboxSize(1366,768),new ZboxSize(1366-SubstractWidth,768-SubstractHeight)},
               {new ZboxSize(1280,1024),new ZboxSize(1280-SubstractWidth,1024-SubstractHeight)},
               {new ZboxSize(1024,768),new ZboxSize(1024-SubstractWidth,768-SubstractHeight)}
        };

        public ImagePreview(IBlobProvider blobProvider,
            IFileProcessorFactory fileProcessorFactory)
        {
            m_BlobProvider = blobProvider;
            m_FileProcessorFactory = fileProcessorFactory;
        }

        public async Task<IEnumerable<string>> GenerateImagePreview(string blobName, ZboxSize screenWidthSize)
        {
            ZboxSize previewImageSize = GetPreviewImageSize(screenWidthSize);
            var cacheBlobName = generatePreviewFileName(blobName, previewImageSize);

            //if (m_BlobProvider.CheckIfFileExistsInCache(cacheBlobName))
            //{
            //    return new List<string> { cacheBlobName };
            //}

            var processor2 = m_FileProcessorFactory.GetProcessor(blobName);
            return await processor2.ConvertFileToWebSitePreview(blobName, previewImageSize.Width, previewImageSize.Height, 0);
            //var stream2 = m_BlobProvider.DownloadFile(blobName);
            //var resizedImage = processor2.ConvertFileToWebSitePreview(stream2, previewImageSize.Width, previewImageSize.Height);

            //m_BlobProvider.UploadFileToCache(cacheBlobName, resizedImage, "image/jpg", false);
            //   return cacheBlobName;
        }

        private ZboxSize GetPreviewImageSize(ZboxSize userScreenWidth)
        {
            if (userScreenWidth == null)
            {
                return new ZboxSize(1024, 768);
            }
            var key = m_PreviewDimenstion.Keys.FirstOrDefault(f => f.Width <= userScreenWidth.Width && f.Height <= userScreenWidth.Height);

            if (key == null)
            {
                return new ZboxSize(1024, 768);
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