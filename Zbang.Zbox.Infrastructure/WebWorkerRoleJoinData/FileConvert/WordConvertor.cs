using Zbang.Zbox.Infrastructure.Storage;
using System.IO;
using System.Drawing;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.FileConvert
{
    public class WordConvertor : Convertor
    {
        public WordConvertor(IBlobProvider blobProvider, string blobName) : base(blobProvider, blobName) { }

        public override void ConvertFileToWebSitePreview()
        {
            var stream = ReadBlob();
            SetLicense();

            var word = new Aspose.Words.Document(stream);
            var saveOprtion = new Aspose.Words.Saving.PdfSaveOptions();
               
            using (var ms = new MemoryStream())
            {
                word.Save(ms, Aspose.Words.Saving.SaveOptions.CreateSaveOptions(Aspose.Words.SaveFormat.Pdf));
                SaveToBlob(ms);
            }
            
        }
        private void SetLicense()
        {
            var license = new Aspose.Words.License();
            license.SetLicense("Aspose.Total.lic");
        }
        public override Image ConvertFileToImage()
        {
            var stream = ReadBlob();
            SetLicense();
            var word = new Aspose.Words.Document(stream);
            using (var ms = new MemoryStream())
            {
                word.Save(ms, Aspose.Words.Saving.SaveOptions.CreateSaveOptions(Aspose.Words.SaveFormat.Jpeg));
                return Image.FromStream(ms);
            }

        }
    }
}
