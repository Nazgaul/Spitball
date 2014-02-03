using Aspose.Pdf;
using Aspose.Pdf.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.FileConvert
{
    public class PdfConvertor : Convertor
    {
        public PdfConvertor(IBlobProvider blobProvider, string blobName) : base(blobProvider, blobName) { }

        public override void ConvertFileToWebSitePreview()
        {
            var stream = ReadBlob();
            SaveToBlob(stream);
        }
        private void SetLicense()
        {
            var license = new Aspose.Pdf.License();
            license.SetLicense("Aspose.Total.lic");
        }

        public override System.Drawing.Image ConvertFileToImage()
        {
            var stream = ReadBlob(true);
            stream.Seek(0, SeekOrigin.Begin);
            SetLicense();

            Document pdfDocument = new Document(stream);


            //using (FileStream imageStream = new FileStream("image.jpg",FileMode.Create))
            //{
            //create JPEG device with specified attributes
            //Width, Height, Resolution, Quality
            //Quality [0-100], 100 is Maximum
            //create Resolution object
            //Resolution resolution = new Resolution(300);
            //JpegDevice jpegDevice = new JpegDevice(500, 700, resolution, 
            //100);

            JpegDevice jpegDevice = new JpegDevice(ThumbnailProvider.SizeOfThumbnail.Width,ThumbnailProvider.SizeOfThumbnail.Height);

            //convert a particular page and save the image to stream
            //jpegDevice.Process(pdfDocument.Pages[0], imageStream);
            using (var ms = new MemoryStream())
            {
                jpegDevice.Process(pdfDocument.Pages[1], ms);
                return System.Drawing.Image.FromStream(ms);
            }
            //close stream
            //imageStream.Close();

            //throw new NotImplementedException();
        }
    }
}
