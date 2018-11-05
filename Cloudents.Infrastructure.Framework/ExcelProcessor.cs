using Aspose.Cells;
using Aspose.Cells.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using License = Aspose.Pdf.License;

namespace Cloudents.Infrastructure.Framework
{
    public class ExcelProcessor : IPreviewProvider2
    {
        public ExcelProcessor()

        {
            using (var sr = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cloudents.Infrastructure.Framework.Aspose.Total.lic"))
            {
                var license = new License();
                license.SetLicense(sr);
            }
        }

        private static void ScalePageSetupToFitPage(Worksheet workSheet)
        {
            workSheet.PageSetup.Orientation = PageOrientationType.Landscape;
            workSheet.PageSetup.FitToPagesTall = 1;
            workSheet.PageSetup.FitToPagesWide = 0;
            workSheet.PageSetup.PaperSize = PaperSizeType.PaperA4;
        }


        public static readonly string[] ExcelExtensions = { ".xls", ".xlsx", ".xlsm", ".xltx", ".ods", ".csv" };
        public async Task ProcessFilesAsync(MemoryStream stream,
            Func<Stream, string, Task> pagePreviewCallback,
            Func<string, Task> textCallback,
            Func<int, Task> pageCountCallback,
            CancellationToken token)
        {
            var excel = new Workbook(stream);
            var imgOptions = new ImageOrPrintOptions { ImageFormat = ImageFormat.Jpeg, OnePagePerSheet = false };

            await pageCountCallback(excel.Worksheets.Count);
            var t = new List<Task>();
            for (int i = 0; i < excel.Worksheets.Count; i++)
            {
                var wb = excel.Worksheets[i];
                ScalePageSetupToFitPage(wb);
                var sr = new SheetRender(wb, imgOptions);
                using (var img = sr.ToImage(0))
                {
                    if (img == null)
                    {
                        continue;
                    }

                    var ms = new MemoryStream();

                    img.Save(ms, ImageFormat.Jpeg);
                    t.Add(pagePreviewCallback(ms, $"{i}.jpg").ContinueWith(_ => ms.Dispose(), token));

                }
            }

            await Task.WhenAll(t);
        }
    }
}
