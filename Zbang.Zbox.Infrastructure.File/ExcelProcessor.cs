﻿using System.Drawing.Imaging;
using System.Threading;
using Aspose.Cells;
using Aspose.Cells.Rendering;
using ImageResizer;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class ExcelProcessor : DocumentProcessor
    {

        const string CacheVersion = CacheVersionPrefix + "5";
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

            var excel = new AsyncLazy<Workbook>(async () =>
            {
                SetLicense();
                using (var sr = await BlobProvider.DownloadFileAsync(blobName, cancelToken))
                {
                    return new Workbook(sr);
                }
            });

            var imgOptions = new ImageOrPrintOptions { ImageFormat = ImageFormat.Jpeg, OnePagePerSheet = false };
            var retVal = await UploadPreviewToAzure(blobName, indexNum, 
                i => CreateCacheFileName(blobName, i),
               async z =>
               {
                   var p = await excel;
                   var workSheet = p.Worksheets[z];
                   ScalePageSetupToFitPage(workSheet);

                   var sr = new SheetRender(workSheet, imgOptions);
                   //Render the image for the sheet
                   var ms = new MemoryStream();

                   sr.ToImage(0, ms);
                   return ms;
               }, CacheVersion,"image/jpg"
            );
            return new PreviewResult { Content = retVal, ViewName = "Image" };
           
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
                var path = await BlobProvider.DownloadToFileAsync(blobName, cancelToken);
                SetLicense();
                var excel = new Workbook(path);
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

                using (var output = new MemoryStream())
                {
                    ImageBuilder.Current.Build(img, output, settings);
                    var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV3.jpg";
                    await BlobProvider.UploadFileThumbnailAsync(thumbnailBlobAddressUri, output, "image/jpeg", cancelToken);
                    return new PreProcessFileResult { ThumbnailName = thumbnailBlobAddressUri };
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
            return DefaultPicture.ExcelFileTypePicture;
        }

        public override Task<string> ExtractContent(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }
    }
}
