using System.Drawing;
using System.Threading;
using Aspose.Imaging;
using Aspose.Imaging.FileFormats.Jpeg;
using Aspose.Imaging.FileFormats.Tiff;
using Aspose.Imaging.ImageOptions;
using Aspose.Imaging.Sources;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.Trace;
using Image = Aspose.Imaging.Image;

namespace Zbang.Zbox.Infrastructure.File
{
    class TiffProcessor : FileProcessor
    {
        public TiffProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {

        }
        private static void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }
        public async override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            var indexOfPageGenerate = CalculateTillWhenToDrawPictures(indexNum);
            Stream blobStr = null;
            var tiff = new Lazy<TiffImage>(() =>
            {
                SetLicense();
                blobStr = BlobProvider.DownloadFile(blobName);

                var tiffImage = (TiffImage)Image.Load(blobStr);
                return tiffImage;


            });
            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task<string>>();
            var jpgCreateOptions = new JpegOptions();


            for (var pageIndex = indexNum; pageIndex < indexOfPageGenerate; pageIndex++)
            {
                var cacheblobName = CreateCacheFileName(blobName, pageIndex);
                var cacheBlobNameWithSharedAccessSignature = BlobProvider.GenerateSharedAccressReadPermissionInCache(cacheblobName, 20);
                if (!string.IsNullOrEmpty(cacheBlobNameWithSharedAccessSignature))
                {
                    blobsNamesInCache.Add(cacheBlobNameWithSharedAccessSignature);
                    continue;
                }
                try
                {


                    tiff.Value.ActiveFrame = tiff.Value.Frames[pageIndex];// tiffFrame;
                    //Load Pixels of TiffFrame into an array of Colors
                    var pixels = tiff.Value.LoadPixels(tiff.Value.Bounds);

                    //Set the Source of bmpCreateOptions as FileCreateSource by specifying the location where output will be saved
                    using (var ms = new MemoryStream())
                    {
                        jpgCreateOptions.Source = new StreamSource(ms);
                        using (var jpgImage =
                  (JpegImage)Image.Create(jpgCreateOptions, tiff.Value.Width, tiff.Value.Height))
                        {
                            //Save the bmpImage with pixels from TiffFrame
                            jpgImage.SavePixels(tiff.Value.Bounds, pixels);
                            jpgImage.Save();
                        }
                        var compressor = new Compress();
                        var gzipSr = compressor.CompressToGzip(ms);
                        parallelTask.Add(BlobProvider.UploadFileToCacheAsync(cacheblobName, gzipSr, "image/jpg", true));
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }

            }
            await Task.WhenAll(parallelTask);
            blobsNamesInCache.AddRange(parallelTask.Select(s => s.Result));
            if (tiff.IsValueCreated)
            {
                tiff.Value.Dispose();
                blobStr.Dispose();
            }
            return new PreviewResult { Content = blobsNamesInCache, ViewName = "Image" };
        }

        protected string CreateCacheFileName(string blobName, int index)
        {
            return string.Format("{0}V4_{2}_{1}.jpg", Path.GetFileNameWithoutExtension(blobName), Path.GetExtension(blobName), index);
        }


        public static readonly string[] TiffExtensions = { ".tiff", ".tif" };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl))
            {
                return TiffExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;

        }

        public override Task<PreProcessFileResult> PreProcessFile(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                var settings = new ResizeSettings
                {
                    Scale = ScaleMode.UpscaleCanvas,
                    Anchor = ContentAlignment.MiddleCenter,
                    BackgroundColor = System.Drawing.Color.White,
                    Mode = FitMode.Crop,
                    Width = ThumbnailWidth,
                    Height = ThumbnailHeight,
                    Quality = 80,
                    Format = "jpg"
                };

                using (var ms = BlobProvider.DownloadFile(blobName))
                {
                    using (var output = new MemoryStream())
                    {
                        ImageBuilder.Current.Build(ms, output, settings);
                        var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV3.jpg";
                        BlobProvider.UploadFileThumbnail(thumbnailBlobAddressUri, output, "image/jpeg");
                        return Task.FromResult(new PreProcessFileResult { ThumbnailName = thumbnailBlobAddressUri });
                    }
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile tiff", ex);
                return Task.FromResult(new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() });
            }
        }

        public override string GetDefaultThumbnailPicture()
        {
            return ThumbnailProvider.ImageFileTypePicture;
        }
    }
}
