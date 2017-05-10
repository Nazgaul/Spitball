using System.Threading;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Pdf.Text;
using Aspose.Pdf.Text.TextOptions;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class PdfProcessor : DocumentProcessor
    {
        private const string CacheVersion = CacheVersionPrefix + "4";

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



        public override async Task<PreviewResult> ConvertFileToWebsitePreviewAsync(Uri blobUri, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];


            var resolution = new Resolution(150);
            var jpegDevice = new JpegDevice(resolution, 90);
            Stream blobSr = null;

            var pdf = new AsyncLazy<Document>(async () =>
            {
                SetLicense();
                blobSr = await BlobProvider.DownloadFileAsync(blobUri, cancelToken).ConfigureAwait(false);
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
                }, CacheVersion, "image/jpg", cancelToken).ConfigureAwait(false);

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
                var path = await BlobProvider.DownloadToLocalDiskAsync(blobUri, cancelToken).ConfigureAwait(false);

                using (var pdfDocument = new Document(path))
                {
                    return await ProcessFileAsync(blobUri, () =>
                        {
                            var jpegDevice = new JpegDevice(new Resolution(150), 80);
                            var ms = new MemoryStream();
                            // ReSharper disable once AccessToDisposedClosure
                            jpegDevice.Process(pdfDocument.Pages[1], ms);
                            return ms;

                        },
                        // ReSharper disable once AccessToDisposedClosure
                        () => pdfDocument.Pages.Count, CacheVersion, cancelToken).ConfigureAwait(false);

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
            var textAbsorber = new TextAbsorber
            {
                ExtractionOptions = new TextExtractionOptions(TextExtractionOptions.TextFormattingMode.Pure)
            };
            for (var i = 1; i <= Math.Min(doc.Pages.Count, 20); i++)
            {
                doc.Pages[i].Accept(textAbsorber);
            }
            var text = textAbsorber.Text;
            var str = StripUnwantedChars(text);
            return str;
        }




        public override async Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            SetLicense();
            var path = await BlobProvider.DownloadToLocalDiskAsync(blobUri, cancelToken).ConfigureAwait(false);
            using (var pdfDocument = new Document(path))
            {
                return ExtractPdfText(pdfDocument);
            }
        }



    }
}
