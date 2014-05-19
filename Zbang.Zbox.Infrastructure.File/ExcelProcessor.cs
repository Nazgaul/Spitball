using Aspose.Cells;
using Aspose.Cells.Rendering;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
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
            var license = new Aspose.Cells.License();
            license.SetLicense("Aspose.Total.lic");
        }



        public async override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum)
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            var indexOfPageGenerate = CalculateTillWhenToDrawPictures(indexNum);

            var excel = new Lazy<Workbook>(() =>
            {
                SetLicense();
                using (var sr = m_BlobProvider.DownloadFile(blobName))
                {
                    return new Workbook(sr);
                }
            });

            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task<string>>();
            var tasks = new List<Task>();

            Aspose.Cells.Rendering.ImageOrPrintOptions imgOptions = new Aspose.Cells.Rendering.ImageOrPrintOptions();
            imgOptions.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
            imgOptions.OnePagePerSheet = false;

            var meta = await m_BlobProvider.FetechBlobMetaDataAsync(blobName);
            for (int pageIndex = indexNum; pageIndex < indexOfPageGenerate; pageIndex++)
            {
                string value;
                var metaDataKey = CacheVersion + pageIndex;
                var cacheblobName = CreateCacheFileName(blobName, pageIndex);

                if (meta.TryGetValue(metaDataKey, out value))
                {
                    blobsNamesInCache.Add(m_BlobProvider.GenerateSharedAccressReadPermissionInCacheWithoutMeta(cacheblobName, 20));
                    meta[metaDataKey] = DateTime.UtcNow.ToFileTimeUtc().ToString();// DateTime.UtcNow.ToString();
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

                    SheetRender sr = new SheetRender(workSheet, imgOptions);
                    //Render the image for the sheet
                    using (var ms = new MemoryStream())
                    {
                        sr.ToImage(0, ms);
                        if (ms.Length == 0)
                        {
                            break;
                        }
                        Compress compressor = new Compress();
                        var gzipSr = compressor.CompressToGzip(ms);

                        parallelTask.Add(m_BlobProvider.UploadFileToCacheAsync(cacheblobName, gzipSr, "image/jpg", true));
                        meta.Add(metaDataKey, DateTime.UtcNow.ToFileTimeUtc().ToString());

                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
            }
            var t = m_BlobProvider.SaveMetaDataToBlobAsync(blobName, meta);
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
       

        public static readonly string[] excelExtensions = { ".xls", ".xlsx", ".xlsm", ".xltx", ".ods", ".csv" };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(m_BlobProvider.BlobContainerUrl))
            {
                return excelExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;

        }

        public override Task<PreProcessFileResult> PreProcessFile(Uri blobUri)
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                using (var stream = m_BlobProvider.DownloadFile(blobName))
                {
                    SetLicense();
                    var excel = new Aspose.Cells.Workbook(stream);
                    var wb = excel.Worksheets[0];
                    //ScalePageSetupToFitPage(wb);

                    Aspose.Cells.Rendering.ImageOrPrintOptions imgOptions = new Aspose.Cells.Rendering.ImageOrPrintOptions();
                    imgOptions.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    imgOptions.OnePagePerSheet = false;

                    Aspose.Cells.Rendering.SheetRender sr = new Aspose.Cells.Rendering.SheetRender(wb, imgOptions);
                    var img = sr.ToImage(0);

                    ResizeSettings settings = new ResizeSettings();
                    settings.Scale = ScaleMode.UpscaleCanvas;
                    settings.Anchor = ContentAlignment.MiddleCenter;
                    settings.BackgroundColor = Color.White;
                    settings.Mode = FitMode.Crop;
                    settings.Width = ThumbnailWidth;
                    settings.Height = ThumbnailHeight;

                    settings.Quality = 80;
                    settings.Format = "jpg";
                    // ImageResizer.ImageBuilder.Current.Build(img, outputFileName + "2.jpg", settings);

                    using (var output = new MemoryStream())
                    {
                        ImageResizer.ImageBuilder.Current.Build(img, output, settings);
                        var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV3.jpg";
                        m_BlobProvider.UploadFileThumbnail(thumbnailBlobAddressUri, output, "image/jpeg");
                        return Task.FromResult<PreProcessFileResult>(new PreProcessFileResult { ThumbnailName = thumbnailBlobAddressUri });
                    }

                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile excel", ex);
                return Task.FromResult<PreProcessFileResult>(new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() });
            }
            
        }

        public override string GetDefaultThumbnailPicture()
        {
            return Zbang.Zbox.Infrastructure.Thumbnail.ThumbnailProvider.ExcelFileTypePicture;
        }
    }
}
