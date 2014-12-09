using System.Drawing.Imaging;
using System.Globalization;
using System.Threading;
using Aspose.Cells;
using Aspose.Cells.Rendering;
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
    public class ExcelProcessor : FileProcessor
    {

        const string CacheVersion = "V5";
        public ExcelProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {

        }


        private void ScalePageSetupToFitPage(Worksheet workSheet)
        {
            workSheet.PageSetup.Orientation = PageOrientationType.Landscape;
            workSheet.PageSetup.FitToPagesTall = 1;
            workSheet.PageSetup.FitToPagesWide = 0;
            workSheet.PageSetup.PaperSize = PaperSizeType.PaperA4;
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

            var excel = new Lazy<Workbook>(() =>
            {
                SetLicense();
                using (var sr = BlobProvider.DownloadFile(blobName))
                {
                    return new Workbook(sr);
                }
            });

            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task<string>>();
            var tasks = new List<Task>();

            var imgOptions = new ImageOrPrintOptions { ImageFormat = ImageFormat.Jpeg, OnePagePerSheet = false };

            var meta = await BlobProvider.FetechBlobMetaDataAsync(blobName);
            for (var pageIndex = indexNum; pageIndex < indexOfPageGenerate; pageIndex++)
            {
                string value;
                var metaDataKey = CacheVersion + pageIndex;
                var cacheblobName = CreateCacheFileName(blobName, pageIndex);

                if (meta.TryGetValue(metaDataKey, out value))
                {
                    blobsNamesInCache.Add(BlobProvider.GenerateSharedAccressReadPermissionInCacheWithoutMeta(cacheblobName, 20));
                    meta[metaDataKey] = DateTime.UtcNow.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture);// DateTime.UtcNow.ToString();
                    continue;
                }
                //var cacheBlobNameWithSharedAccessSignature = m_BlobProvider.GenerateSharedAccressReadPermissionInCache(cacheblobName, 20);
                //if (!string.IsNullOrEmpty(cacheBlobNameWithSharedAccessSignature))
                //{
                //    blobsNamesInCache.Add(cacheBlobNameWithSharedAccessSignature);
                //    continue;
                //}
                try
                {

                    var workSheet = excel.Value.Worksheets[pageIndex];
                    ScalePageSetupToFitPage(workSheet);

                    var sr = new SheetRender(workSheet, imgOptions);
                    //Render the image for the sheet
                    using (var ms = new MemoryStream())
                    {
                        sr.ToImage(0, ms);
                        if (ms.Length == 0)
                        {
                            break;
                        }
                        var compressor = new Compress();
                        var gzipSr = compressor.CompressToGzip(ms);

                        parallelTask.Add(BlobProvider.UploadFileToCacheAsync(cacheblobName, gzipSr, "image/jpg", true));
                        meta.Add(metaDataKey, DateTime.UtcNow.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture));

                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
            }
            var t = BlobProvider.SaveMetaDataToBlobAsync(blobName, meta);
            tasks.AddRange(parallelTask);
            tasks.Add(t);
            await Task.WhenAll(tasks);
            blobsNamesInCache.AddRange(parallelTask.Select(s => s.Result));

            return new PreviewResult { Content = blobsNamesInCache, ViewName = "Image" };
        }

        protected string CreateCacheFileName(string blobName, int index)
        {
            return string.Format("{0}{3}_{2}_{1}.jpg", Path.GetFileNameWithoutExtension(blobName), Path.GetExtension(blobName), index, CacheVersion);
        }


        public static readonly string[] ExcelExtensions = { ".xls", ".xlsx", ".xlsm", ".xltx", ".ods", ".csv" };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl))
            {
                return ExcelExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
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
                    SetLicense();
                    var excel = new Workbook(stream);
                    var wb = excel.Worksheets[0];
                    //ScalePageSetupToFitPage(wb);

                    var imgOptions = new ImageOrPrintOptions { ImageFormat = ImageFormat.Jpeg, OnePagePerSheet = false };

                    var sr = new SheetRender(wb, imgOptions);
                    var img = sr.ToImage(0);
                    if (img == null)
                    {
                        return new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() };
                    }
                    var settings = new ResizeSettings
                    {
                        Scale = ScaleMode.UpscaleCanvas,
                        Anchor = ContentAlignment.MiddleCenter,
                        BackgroundColor = Color.White,
                        Mode = FitMode.Crop,
                        Width = ThumbnailWidth,
                        Height = ThumbnailHeight,
                        Quality = 80,
                        Format = "jpg"
                    };

                    // ImageResizer.ImageBuilder.Current.Build(img, outputFileName + "2.jpg", settings);

                    using (var output = new MemoryStream())
                    {
                        ImageBuilder.Current.Build(img, output, settings);
                        var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV3.jpg";
                        await BlobProvider.UploadFileThumbnailAsync(thumbnailBlobAddressUri, output, "image/jpeg");
                        return new PreProcessFileResult { ThumbnailName = thumbnailBlobAddressUri };
                    }

                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile excel", ex);
                return new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() };
            }

        }

        public override string GetDefaultThumbnailPicture()
        {
            return ThumbnailProvider.ExcelFileTypePicture;
        }
    }
}
