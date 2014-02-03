using Aspose.Imaging;
using Aspose.Imaging.FileFormats.Tiff;
using Aspose.Imaging.ImageOptions;
using Aspose.Imaging.Sources;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    class TiffProcessor : FileProcessor, IContentProcessor
    {
        public TiffProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {

        }
        private static void SetLicense()
        {
            var license = new Aspose.Imaging.License();
            license.SetLicense("Aspose.Total.lic");
        }
        public async override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum)
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            var indexOfPageGenerate = CalculateTillWhenToDrawPictures(indexNum);
            Stream blobStr = null;
            var tiff = new Lazy<TiffImage>(() =>
            {
                SetLicense();
                blobStr = m_BlobProvider.DownloadFile(blobName);

                var tiffImage = (TiffImage)Image.Load(blobStr);
                return tiffImage;


            });
            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task<string>>();
            JpegOptions jpgCreateOptions = new JpegOptions();


            for (int pageIndex = indexNum; pageIndex < indexOfPageGenerate; pageIndex++)
            {
                var cacheblobName = CreateCacheFileName(blobName, pageIndex);
                var cacheBlobNameWithSharedAccessSignature = m_BlobProvider.GenerateSharedAccressReadPermissionInCache(cacheblobName, 20);
                if (!string.IsNullOrEmpty(cacheBlobNameWithSharedAccessSignature))
                {
                    blobsNamesInCache.Add(cacheBlobNameWithSharedAccessSignature);
                    continue;
                }
                try
                {


                    tiff.Value.ActiveFrame = tiff.Value.Frames[pageIndex];// tiffFrame;
                    //Load Pixels of TiffFrame into an array of Colors
                    Color[] pixels = tiff.Value.LoadPixels(tiff.Value.Bounds);

                    //Set the Source of bmpCreateOptions as FileCreateSource by specifying the location where output will be saved
                    using (var ms = new MemoryStream())
                    {
                        jpgCreateOptions.Source = new Aspose.Imaging.Sources.StreamSource(ms);
                        using (Aspose.Imaging.FileFormats.Jpeg.JpegImage jpgImage =
                  (Aspose.Imaging.FileFormats.Jpeg.JpegImage)Image.Create(jpgCreateOptions, tiff.Value.Width, tiff.Value.Height))
                        {
                            //Save the bmpImage with pixels from TiffFrame
                            jpgImage.SavePixels(tiff.Value.Bounds, pixels);
                            jpgImage.Save();
                        }
                        Compress compressor = new Compress();
                        var gzipSr = compressor.CompressToGzip(ms);
                        parallelTask.Add(m_BlobProvider.UploadFileToCacheAsync(cacheblobName, gzipSr, "image/jpg", true));
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


        public static readonly string[] tiffExtesions = { ".tiff", ".tif" };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl))
            {
                return tiffExtesions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;

        }

        public override Task<PreProcessFileResult> PreProcessFile(Uri blobUri)
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                ResizeSettings settings = new ResizeSettings();
                settings.Scale = ScaleMode.UpscaleCanvas;
                settings.Anchor = System.Drawing.ContentAlignment.MiddleCenter;
                settings.BackgroundColor = System.Drawing.Color.White;
                settings.Mode = FitMode.Crop;
                settings.Width = ThumbnailWidth;
                settings.Height = ThumbnailHeight;

                settings.Quality = 80;
                settings.Format = "jpg";

                using (var ms = m_BlobProvider.DownloadFile(blobName))
                {
                    using (var output = new MemoryStream())
                    {
                        ImageResizer.ImageBuilder.Current.Build(ms, output, settings);
                        var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV3.jpg";
                        m_BlobProvider.UploadFileThumbnail(thumbnailBlobAddressUri, output, "image/jpeg");
                        return Task.FromResult<PreProcessFileResult>(new PreProcessFileResult { ThumbnailName = thumbnailBlobAddressUri });
                    }
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile tiff", ex);
                return Task.FromResult<PreProcessFileResult>(new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() });
            }
        }

        public override string GetDefaultThumbnailPicture()
        {
            return Zbang.Zbox.Infrastructure.Thumbnail.ThumbnailProvider.ImageFileTypePicture;
        }
    }
}
