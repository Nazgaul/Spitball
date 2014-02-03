using Zbang.Zbox.Infrastructure.Storage;
using System.IO;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.FileConvert
{
    public class PowerPointConvertor : Convertor
    {
        public PowerPointConvertor(IBlobProvider blobProvider, string blobName) : base(blobProvider, blobName) { }
        public override void ConvertFileToWebSitePreview()
        {
            var stream = ReadBlob();
            SetLicense();

            var ppt = new Aspose.Slides.Presentation(stream);

            using (var ms = new MemoryStream())
            {
                ppt.Save(ms, Aspose.Slides.Export.SaveFormat.Pdf);
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
            var ppt = new Aspose.Slides.Presentation(stream);
            return ppt.Slides[0].GetThumbnail(1,1);
        }
    }
}
