using Zbang.Zbox.Infrastructure.Storage;
using System.IO;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.FileConvert
{
    public class PowerPoint2007Convertor : Convertor
    {
        public PowerPoint2007Convertor(IBlobProvider blobProvider, string blobName) : base(blobProvider, blobName) { }
        public override void ConvertFileToWebSitePreview()
        {
            var stream = ReadBlob();
            SetLicense();

            var pptx = new Aspose.Slides.Pptx.PresentationEx(stream);
            
            using (var ms = new MemoryStream())
            {
                pptx.Save(ms, Aspose.Slides.Export.SaveFormat.Pdf);
                SaveToBlob(ms);
            }
        }

        private static void SetLicense()
        {
            var license = new Aspose.Slides.License();
            license.SetLicense("Aspose.Total.lic");
        }

        public override System.Drawing.Image ConvertFileToImage()
        {
            var stream = ReadBlob();
            SetLicense();
            var pptx = new Aspose.Slides.Pptx.PresentationEx(stream);
            return pptx.Slides[0].GetThumbnail(1,1);
        }
    }
}
