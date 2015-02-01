using System.Text;
using System.Threading;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Pdf.Text.TextOptions;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class PdfProcessor : DocumentProcessor
    {

        const string CacheVersion = CacheVersionPrefix + "4";

        public PdfProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {

        }


        private void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }



        public async override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];


            var resolution = new Resolution(150);
            var jpegDevice = new JpegDevice(resolution, 90);
            Stream blobSr = null;

            var pdf = new AsyncLazy<Document>(async () =>
            {
                SetLicense();
                blobSr = await BlobProvider.DownloadFileAsync(blobName, cancelToken);
                return new Document(blobSr);
            });

            var retVal = await UploadPreviewToAzure(blobName,
                ++indexNum,
                i => CreateCacheFileName(blobName, i),
                async z =>
                {
                    var ms = new MemoryStream();
                    var p = await pdf;
                    jpegDevice.Process(p.Pages[z], ms);
                    return ms;
                }, CacheVersion, "image/jpg");

            if (pdf.IsValueCreated && blobSr != null)
            {
                blobSr.Dispose();
                pdf.Value.Dispose();
            }
            return new PreviewResult { Content = retVal, ViewName = "Image" };
        }
        protected string CreateCacheFileName(string blobName, int index)
        {
            return string.Format("{0}{3}_{2}_{1}.jpg", Path.GetFileNameWithoutExtension(blobName), Path.GetExtension(blobName), index, CacheVersion);
        }



        public static readonly string[] PdfExtensions = { ".pdf" };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl))
            {
                return PdfExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;

        }


        public override async Task<PreProcessFileResult> PreProcessFile(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                SetLicense();
                var path = await BlobProvider.DownloadToFileAsync(blobName, cancelToken);



                using (var pdfDocument = new Document(path))
                {
                    return await ProcessFile(blobName, () =>
                     {
                         var jpegDevice = new JpegDevice(ThumbnailWidth, ThumbnailHeight, new Resolution(150), 80);
                         var ms = new MemoryStream();
                         jpegDevice.Process(pdfDocument.Pages[1], ms);
                         return ms;

                     }, () => ExtractPdfText(pdfDocument),
                     () => pdfDocument.Pages.Count, CacheVersion

                     );
                   
                }

            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile pdf", ex);
                return new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() };
            }
        }






        private string ExtractPdfText(Document doc)
        {
            try
            {
                var builder = new StringBuilder();
                //string to hold extracted text
                string extractedText = "";
                foreach (Page pdfPage in doc.Pages)
                {

                    using (var textStream = new MemoryStream())
                    {
                        //create text device
                        var textDevice = new TextDevice();

                        //set text extraction options - set text extraction mode (Raw or Pure)
                        var textExtOptions = new
                        TextExtractionOptions(TextExtractionOptions.TextFormattingMode.Pure);
                        textDevice.ExtractionOptions = textExtOptions;

                        //convert a particular page and save text to the stream
                        if (pdfPage != null) textDevice.Process(pdfPage, textStream);

                        //close memory stream
                        textStream.Close();

                        //get text from memory stream
                        extractedText = Encoding.Unicode.GetString(textStream.ToArray());
                        extractedText = StripUnwantedChars(extractedText);
                    }
                    builder.Append(extractedText);
                    if (builder.Length > 10000)
                    {
                        break;
                    }
                }
                //var textAbsorber = new TextAbsorber();
                //for (var i = 1; i < Math.Min(doc.Pages.Count, 10); i++)
                //{
                //    doc.Pages[i].Accept(textAbsorber);
                //}
                //var str = textAbsorber.Text;
                var str = StripUnwantedChars(builder.ToString());
                return str;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("trying to extract pdf text", ex);
                return string.Empty;
            }
        }
        public override string GetDefaultThumbnailPicture()
        {
            return ThumbnailProvider.PdfFileTypePicture;
        }


    }
}
