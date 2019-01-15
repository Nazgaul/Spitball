﻿using Aspose.Cells;
using Aspose.Cells.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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


        Workbook _excel;

        public void Init(Stream stream)
        {
            _excel = new Workbook(stream);
        }

        public (string text, int pagesCount) ExtractMetaContent()
        {
            
            return (null, _excel.Worksheets.Count);
        }

      

        public static readonly string[] Extensions = { ".xls", ".xlsx", ".xlsm", ".xltx", ".ods", ".csv" };
        public async Task ProcessFilesAsync(IEnumerable<int> previewDelta, Func<Stream, string, Task> pagePreviewCallback,
            CancellationToken token)
        {
            var imgOptions = new ImageOrPrintOptions { ImageFormat = ImageFormat.Jpeg, OnePagePerSheet = false };

            var t = new List<Task>();

            var diff = Enumerable.Range(0, _excel.Worksheets.Count);
            diff = diff.Except(previewDelta);
            
            foreach (var item in diff)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                var wb = _excel.Worksheets[item];
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
                    t.Add(pagePreviewCallback(ms, $"{item}.jpg").ContinueWith(_ => ms.Dispose(), token));
                }
            }
            await Task.WhenAll(t);
        }
    }
}
