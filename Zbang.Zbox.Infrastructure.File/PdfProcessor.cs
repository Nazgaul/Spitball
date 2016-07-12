﻿using System.Text;
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
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class PdfProcessor : DocumentProcessor
    {

        const string CacheVersion = CacheVersionPrefix + "4";

        public PdfProcessor(
            IBlobProvider blobProvider,
            IBlobProvider2<IPreviewContainer> blobProviderPreview,
            IBlobProvider2<ICacheContainer> blobProviderCache)
            : base(blobProvider, blobProviderPreview, blobProviderCache)
        {
            SetLicense();
        }


        private static void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }



        public override async Task<PreviewResult> ConvertFileToWebSitePreviewAsync(Uri blobUri, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];


            var resolution = new Resolution(150);
            var jpegDevice = new JpegDevice(resolution, 90);
            Stream blobSr = null;

            var pdf = new AsyncLazy<Document>(async () =>
            {
                SetLicense();
                blobSr = await BlobProvider.DownloadFileAsync(blobUri, cancelToken);
                return new Document(blobSr);
            });

            var retVal = await UploadPreviewCacheToAzureAsync(blobUri,
                indexNum,
                i => CreateCacheFileName(blobName, i),
                async z =>
                {
                    var ms = new MemoryStream();
                    var p = await pdf;
                    jpegDevice.Process(p.Pages[z + 1], ms);
                    return ms;
                }, CacheVersion, "image/jpg", cancelToken);

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
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.StorageContainerUrl))
            {
                return PdfExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;

        }


        public override async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            try
            {
                SetLicense();
                var path = await BlobProvider.DownloadToLocalDiskAsync(blobUri, cancelToken);



                using (var pdfDocument = new Document(path))
                {
                    return await ProcessFileAsync(blobUri, () =>
                     {
                         var jpegDevice = new JpegDevice(new Resolution(150), 80);
                         var ms = new MemoryStream();
                         jpegDevice.Process(pdfDocument.Pages[1], ms);
                         return ms;

                     }, 
                     () => pdfDocument.Pages.Count, CacheVersion, cancelToken);

                }

            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile pdf", ex);
                return null;
            }
        }






        private string ExtractPdfText(Document doc)
        {
            try
            {
                var builder = new StringBuilder();
                //string to hold extracted text
                for (int i = 1; i <= Math.Min(doc.Pages.Count, 20); i++)
                {
                    var pdfPage = doc.Pages[i];
                    string extractedText;
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
                }
                var str = StripUnwantedChars(builder.ToString());
                return str;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("trying to extract pdf text", ex);
                return string.Empty;
            }
        }




        public override async Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = GetBlobNameFromUri(blobUri);
            SetLicense();
            var path = await BlobProvider.DownloadToLocalDiskAsync(blobUri, cancelToken);
            using (var pdfDocument = new Document(path))
            {
                return ExtractPdfText(pdfDocument);
            }
        }

    }
}
