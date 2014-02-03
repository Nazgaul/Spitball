
using Zbang.Zbox.Infrastructure.Storage;
using System.IO;
using Aspose.Cells;
using System.Drawing;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.FileConvert
{
    public class ExcelConvertor : Convertor
    {
        public ExcelConvertor(IBlobProvider blobProvider, string blobName) : base(blobProvider, blobName) { }

        public override void ConvertFileToWebSitePreview()
        {
            var stream = ReadBlob();
            SetLicense();
            var excel = new Aspose.Cells.Workbook(stream);
            foreach (Worksheet workSheet in excel.Worksheets)
            {
                ScalePageSetupToFitPage(workSheet);
            }
            var p = new PdfSaveOptions();
            using (var ms = new MemoryStream())
            {
                excel.Save(ms, p);
                SaveToBlob(ms);
            }
        }

        private static void SetLicense()
        {
            var license = new Aspose.Cells.License();
            license.SetLicense("Aspose.Total.lic");
        }

        private static void ScalePageSetupToFitPage(Worksheet workSheet)
        {
            workSheet.PageSetup.Orientation = PageOrientationType.Landscape;
            workSheet.PageSetup.FitToPagesTall = 0;
            workSheet.PageSetup.FitToPagesWide = 1;
            workSheet.PageSetup.PaperSize = PaperSizeType.PaperA4;
        }


        public override Image ConvertFileToImage()
        {
            var stream = ReadBlob();
            SetLicense();
            var excel = new Aspose.Cells.Workbook(stream);
            var wb = excel.Worksheets[0];
            ScalePageSetupToFitPage(wb);

            Aspose.Cells.Rendering.ImageOrPrintOptions imgOptions = new Aspose.Cells.Rendering.ImageOrPrintOptions();
            imgOptions.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
            imgOptions.OnePagePerSheet = false;

            Aspose.Cells.Rendering.SheetRender sr = new Aspose.Cells.Rendering.SheetRender(wb, imgOptions);
            var img = sr.ToImage(0);

            return img;

        }
    }
}
