using System.Threading;
using Aspose.Cells;
using Aspose.Cells.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Framework
{
    public class ExcelProcessor : Processor, IPreviewProvider
    {
        private const string CacheVersion = CacheVersionPrefix + "5";
        public ExcelProcessor(Uri blobUri,
            IBlobProvider blobProvider,
            IBlobProvider<CacheContainer> blobProviderCache)
            : base(blobProvider, blobProviderCache, blobUri)
        {
            SetLicense();
        }

        private static void ScalePageSetupToFitPage(Worksheet workSheet)
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

        public Task<IEnumerable<string>> ConvertFileToWebsitePreviewAsync(int indexNum, CancellationToken cancelToken)
        {
            var blobName = BlobProvider.GetBlobNameFromUri(BlobUri);

            var excel = new AsyncLazy<Workbook>(async () =>
            {
                using (var sr = await BlobProvider.DownloadFileAsync(BlobUri, cancelToken).ConfigureAwait(false))
                {
                    return new Workbook(sr);
                }
            });

            var imgOptions = new ImageOrPrintOptions { ImageFormat = ImageFormat.Jpeg, OnePagePerSheet = false };
            return UploadPreviewCacheToAzureAsync(indexNum,
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
               }, CacheVersion, "image/jpg", cancelToken
            );
        }

        protected static string CreateCacheFileName(string blobName, int index)
        {
            return $"{Path.GetFileNameWithoutExtension(blobName)}{CacheVersion}_{index}_{Path.GetExtension(blobName)}.jpg";
        }

        public static readonly string[] ExcelExtensions = { ".xls", ".xlsx", ".xlsm", ".xltx", ".ods", ".csv" };

        //public override bool CanProcessFile(Uri blobName)
        //{
        //    if (blobName.AbsoluteUri.StartsWith(BlobProvider.StorageContainerUrl))
        //    {
        //        return ExcelExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        //    }
        //    return false;
        //}

        //public override async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        //{
        //    try
        //    {
        //        var path = await BlobProvider.DownloadToLocalDiskAsync(blobUri, cancelToken).ConfigureAwait(false);
        //        SetLicense();
        //        var excel = new Workbook(path);
        //        var wb = excel.Worksheets[0];

        //        return await ProcessFileAsync(blobUri, () =>
        //        {
        //            var imgOptions = new ImageOrPrintOptions { ImageFormat = ImageFormat.Jpeg, OnePagePerSheet = false };
        //            var sr = new SheetRender(wb, imgOptions);
        //            using (var img = sr.ToImage(0))
        //            {
        //                if (img == null)
        //                {
        //                    return null;
        //                }
        //                var ms = new MemoryStream();
        //                img.Save(ms, ImageFormat.Jpeg);
        //                return ms;
        //            }
        //        },  () => excel.Worksheets.Count, CacheVersion, cancelToken).ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("PreProcessFile excel", ex);
        //        return null;
        //    }
        //}

        //public override Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        //{
        //    return Extensions.TaskExtensions.CompletedTaskString;
        //}
    }
}
