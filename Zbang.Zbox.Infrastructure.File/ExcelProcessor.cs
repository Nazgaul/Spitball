using System.Drawing.Imaging;
using System.Threading;
using Aspose.Cells;
using Aspose.Cells.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class ExcelProcessor : DocumentProcessor
    {
        private const string CacheVersion = CacheVersionPrefix + "5";
        public ExcelProcessor(IBlobProvider blobProvider, 
            IBlobProvider2<IPreviewContainer> blobProviderPreview,
            IBlobProvider2<ICacheContainer> blobProviderCache)
            : base(blobProvider, blobProviderPreview, blobProviderCache)
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



        public override async Task<PreviewResult> ConvertFileToWebSitePreviewAsync(Uri blobUri, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];

            var excel = new AsyncLazy<Workbook>(async () =>
            {
                SetLicense();
                using (var sr = await BlobProvider.DownloadFileAsync(blobUri, cancelToken))
                {
                    return new Workbook(sr);
                }
            });

            var imgOptions = new ImageOrPrintOptions { ImageFormat = ImageFormat.Jpeg, OnePagePerSheet = false };
            var retVal = await UploadPreviewCacheToAzureAsync(blobUri, indexNum,
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
            return new PreviewResult { Content = retVal, ViewName = "Image" };

        }

        protected string CreateCacheFileName(string blobName, int index)
        {
            return string.Format("{0}{3}_{2}_{1}.jpg", Path.GetFileNameWithoutExtension(blobName), Path.GetExtension(blobName), index, CacheVersion);
        }


        public static readonly string[] ExcelExtensions = { ".xls", ".xlsx", ".xlsm", ".xltx", ".ods", ".csv" };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.StorageContainerUrl))
            {
                return ExcelExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;

        }

        public override async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            try
            {
                var path = await BlobProvider.DownloadToLocalDiskAsync(blobUri, cancelToken);
                SetLicense();
                var excel = new Workbook(path);
                var wb = excel.Worksheets[0];

                return await ProcessFileAsync(blobUri, () =>
                {
                    var imgOptions = new ImageOrPrintOptions { ImageFormat = ImageFormat.Jpeg, OnePagePerSheet = false };
                    var sr = new SheetRender(wb, imgOptions);
                    using (var img = sr.ToImage(0))
                    {
                        if (img == null)
                        {
                            return null;
                        }
                        var ms = new MemoryStream();
                        img.Save(ms, ImageFormat.Jpeg);
                        return ms;
                    }
                },  () => excel.Worksheets.Count, CacheVersion, cancelToken);

            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile excel", ex);
                return null;
            }

        }

        public override Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            return Extensions.TaskExtensions.CompletedTaskString;
        }

    }
}
