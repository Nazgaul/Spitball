using System.Text;
using System.Threading;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using System;
using System.Collections.Generic;
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
            //var builder = new StringBuilder();
            var pageTexts = new List<string>();
            try
            {

                //string to hold extracted text
                for (var i = 1; i <= Math.Min(doc.Pages.Count, 20); i++)
                {
                    var pdfPage = doc.Pages[i];
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
                        var extractedText = Encoding.Unicode.GetString(textStream.ToArray());
                        if (string.IsNullOrWhiteSpace(extractedText))
                        {
                            continue;
                        }
                        extractedText = StripUnwantedChars(extractedText);


                        if (pageTexts.Contains(extractedText))
                        {
                            continue;
                        }
                        var d = LevenshteinDistance.Compute(pageTexts.LastOrDefault(), extractedText);
                        if (d < 5)
                        {
                            TraceLog.WriteInfo(
                                $"LevenshteinDistance less then 10 between {pageTexts.LastOrDefault()} and  {extractedText}");
                            continue;
                        }
                        pageTexts.Add(extractedText);
                    }
                    //builder.Append(extractedText);
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("trying to extract pdf text", ex);
            }
            var str = StripUnwantedChars(string.Join(" ", pageTexts));
            return str;
        }




        public override async Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            SetLicense();
            var path = await BlobProvider.DownloadToLocalDiskAsync(blobUri, cancelToken);
            using (var pdfDocument = new Document(path))
            {
                return ExtractPdfText(pdfDocument);
            }
        }



    }

    static class LevenshteinDistance
    {
        public static int Compute(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }
    }
}
