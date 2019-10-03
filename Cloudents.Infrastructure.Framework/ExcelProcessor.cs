using Aspose.Cells;
using Aspose.Cells.Rendering;
using Cloudents.Core;
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
    public class ExcelProcessor : IPreviewProvider
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


        private Lazy<Workbook> _excel;

        public void Init(Func<Stream> stream)
        {
            _excel = new Lazy<Workbook>(() => new Workbook(stream()));
        }

        public void Init(Func<string> stream)
        {
            _excel = new Lazy<Workbook>(() => new Workbook(stream()));
        }

        public (string text, int pagesCount) ExtractMetaContent()
        {
            
            return (null, _excel.Value.Worksheets.Count);
        }



        public async Task ProcessFilesAsync(IEnumerable<int> previewDelta, Func<Stream, string, Task> pagePreviewCallback,
            CancellationToken token)
        {
            var imgOptions = new ImageOrPrintOptions { ImageFormat = ImageFormat.Jpeg, OnePagePerSheet = false };

            var t = new List<Task>();

            var diff = Enumerable.Range(0, _excel.Value.Worksheets.Count);
            diff = diff.Except(previewDelta);
            
            foreach (var item in diff)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                var wb = _excel.Value.Worksheets[item];
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

        //public void Init(Func<string> path)
        //{
        //    _excel = new Lazy<Workbook>(
        //         () => {
        //            return new Workbook(path());
        //            }
        //        );
        //}

        
    }


   
}
