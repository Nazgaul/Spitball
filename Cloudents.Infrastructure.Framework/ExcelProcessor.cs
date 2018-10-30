﻿using System;
using System.Threading;
using Aspose.Cells;
using Aspose.Cells.Rendering;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Framework
{
    public class ExcelProcessor :  IPreviewProvider2
    {
        public ExcelProcessor()
            
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

       

     

        public static readonly string[] ExcelExtensions = { ".xls", ".xlsx", ".xlsm", ".xltx", ".ods", ".csv" };
        public async Task CreatePreviewFilesAsync(MemoryStream stream, Func<Stream, string, Task> callback, Func<string, Task> textCallback, CancellationToken token)
        {
            var excel = new Workbook(stream);
            var imgOptions = new ImageOrPrintOptions { ImageFormat = ImageFormat.Jpeg, OnePagePerSheet = false };

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
                    await callback(ms, $"{i}.jpg");
                }
            }
            
        }
    }
}
