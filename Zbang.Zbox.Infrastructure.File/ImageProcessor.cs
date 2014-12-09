using System.Threading;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class ImageProcessor : FileProcessor
    {
        private const int SubstractWidth = 100;
        const int SubstractHeight = 100;

        private readonly Dictionary<Size, Size> m_PreviewDimension = new Dictionary<Size, Size> {
            {new Size(1920,1080),new Size(1920-SubstractWidth,1080-SubstractHeight)},
            {new Size(1440,900),new Size(1440-SubstractWidth,900-SubstractHeight)},
               {new Size(1366,768),new Size(1366-SubstractWidth,768-SubstractHeight)},
               {new Size(1280,1024),new Size(1280-SubstractWidth,1024-SubstractHeight)},
               {new Size(1024,768),new Size(1024-SubstractWidth,768-SubstractHeight)},
               {new Size(320,480),new Size(320-SubstractWidth,480-SubstractHeight)}
        };

        public ImageProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {

        }
        public async override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = GetBlobNameFromUri(blobUri);
            var blobsNamesInCache = new List<string>();

            if (indexNum > 0)
            {
                return new PreviewResult { Content = blobsNamesInCache };
            }
            var imageDimentions = GetPreviewImageSize(new Size(width, height));
            var cacheFileName = CreateCacheFileName(blobName, imageDimentions);

            var cacheBlobNameWithSharedAccessSignature = BlobProvider.GenerateSharedAccressReadPermissionInCache(cacheFileName, 20);

            if (IsFileExistsInCache(cacheBlobNameWithSharedAccessSignature))
            {
                blobsNamesInCache.Add(cacheBlobNameWithSharedAccessSignature);
                return new PreviewResult { ViewName = "Image", Content = blobsNamesInCache };
            }


            using (var stream = await BlobProvider.DownloadFileAsync(blobName))
            {
                if (stream.Length == 0)
                {
                    throw new ArgumentException("Stream is 0");
                }
                var settings = new ResizeSettings {Mode = FitMode.Max};
                using (var outPutStream = ProcessFile(stream, imageDimentions.Width, imageDimentions.Height, settings))
                {
                    var cacheName = await BlobProvider.UploadFileToCacheAsync(cacheFileName, outPutStream, "image/jpg");
                    blobsNamesInCache.Add(cacheName);
                }
            }
            return new PreviewResult { Content = blobsNamesInCache, ViewName = "Image" };
        }

        private Stream ProcessFile(Stream stream, int width, int height, ResizeSettings settings)
        {
            stream.Seek(0, SeekOrigin.Begin);

            settings.Width = width;
            settings.Height = height;

            settings.Quality = 80;
            settings.Format = "jpg";

            var ms = new MemoryStream();
            ImageBuilder.Current.Build(stream, ms, settings);
            return ms;
        }

        private bool IsFileExistsInCache(string cacheBlobNameWithSharedAccessSignature)
        {
            return !string.IsNullOrEmpty(cacheBlobNameWithSharedAccessSignature);
        }


        private string CreateCacheFileName(string blobName, Size dimensions)
        {
            var fileName = string.Format("{0}_{1}*{2}{3}.jpg", Path.GetFileNameWithoutExtension(blobName), dimensions.Width, dimensions.Height, Path.GetExtension(blobName));
            return fileName;
        }

        private Size GetPreviewImageSize(Size userScreenWidth)
        {
            if (userScreenWidth.IsEmpty)
            {
                return new Size(1024, 768);
            }
            var key = m_PreviewDimension.Keys.FirstOrDefault(f => f.Width <= userScreenWidth.Width && f.Height <= userScreenWidth.Height);
            if (key == Size.Empty)
            //if (key == null)
            {
                return new Size(1024, 768);
            }
            return m_PreviewDimension[key];
        }

        public static readonly string[] ImageExtensions = { ".jpg", ".gif", ".png", ".jpeg" , ".bmp" };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl))
            {
                return ImageExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;

        }

        public override async Task<PreProcessFileResult> PreProcessFile(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                using (var stream = await BlobProvider.DownloadFileAsync(blobName))
                {
                    if (stream.Length == 0)
                    {
                        TraceLog.WriteError("image is empty" + blobName);
                        return new PreProcessFileResult {ThumbnailName = GetDefaultThumbnailPicture()};
                    }
                    //var settings = new ResizeSettings
                    //{
                    //    Scale = ScaleMode.UpscaleCanvas,
                    //    Anchor = ContentAlignment.MiddleCenter,
                    //    BackgroundColor = Color.White,
                    //    Mode = FitMode.Crop
                    //};
                    var settings = new ResizeSettings
                    {
                        
                        Anchor = ContentAlignment.MiddleCenter,
                        BackgroundColor = Color.White,
                       // Mode = FitMode.Crop,
                        Scale = ScaleMode.UpscaleCanvas
                    };

                    using (var outPutStream = ProcessFile(stream, ThumbnailWidth, ThumbnailHeight, settings))
                    {
                        var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV4.jpg";
                       await BlobProvider.UploadFileThumbnailAsync(thumbnailBlobAddressUri, outPutStream, "image/jpeg");
                        return new PreProcessFileResult { ThumbnailName = thumbnailBlobAddressUri };
                    }

                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile image", ex);
                return new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() };
            }

        }

        public override string GetDefaultThumbnailPicture()
        {
            return ThumbnailProvider.ImageFileTypePicture;
        }
    }
}
