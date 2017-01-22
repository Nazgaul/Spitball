using System.Text;
using System.Threading;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using System;
using System.Collections.Generic;
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
            var textAbsorber = new TextAbsorber();
            textAbsorber.ExtractionOptions = new TextExtractionOptions(TextExtractionOptions.TextFormattingMode.Pure);
            for (var i = 1; i <= Math.Min(doc.Pages.Count, 20); i++)
            {
                doc.Pages[i].Accept(textAbsorber);
            }

            //doc.Pages.Accept(textAbsorber);
            var text = textAbsorber.Text;
            var pageTexts = new List<string>();
            var lineBreakTexts = text.Split(new[] { "\r\n", "." }, StringSplitOptions.RemoveEmptyEntries)
                            .GroupBy(m => m)
                            .Where(m => m.Count() == 1 && m.Key != string.Empty)
                            .Select(s => s.Key);

            foreach (var lineBreakText in lineBreakTexts)
            {
                var z = TextManipulation.SpaceReg.Split(lineBreakText);
                if (!z.Any(a => a.Length > 1)) continue;
                var txt = StripUnwantedChars(lineBreakText);


                //if (pageTexts.Contains(txt))
                //{
                //    continue;
                //}
                SimMetricsMetricUtilities.JaroWinkler x = new SimMetricsMetricUtilities.JaroWinkler();

                var result = x.BatchCompareSet(pageTexts.ToArray(), txt);
                if (result.Any(w => w > 0.95))
                {
                    continue;
                }

                //var d = z.GetSimilarity(pageTexts.LastOrDefault(), text);
                //var d = LevenshteinDistance.Compute(pageTexts.LastOrDefault(), txt);
                //if (d < 5)
                //{
                //    TraceLog.WriteInfo(
                //        $"LevenshteinDistance less then 10 between {pageTexts.LastOrDefault()} and  {lineBreakText}");
                //    continue;
                //}
                pageTexts.Add(txt);
            }

            //var builder = new StringBuilder();
            //var pageTexts = new List<string>();
            //try
            //{

            //    //string to hold extracted text
            //    for (var i = 1; i <= Math.Min(doc.Pages.Count, 20); i++)
            //    {
            //        var pdfPage = doc.Pages[i];
            //        using (var textStream = new MemoryStream())
            //        {
            //            //create text device
            //            var textDevice = new TextDevice();

            //            //set text extraction options - set text extraction mode (Raw or Pure)
            //            var textExtOptions = new
            //            TextExtractionOptions(TextExtractionOptions.TextFormattingMode.Pure);
            //            textDevice.ExtractionOptions = textExtOptions;

            //            //convert a particular page and save text to the stream
            //            if (pdfPage != null) textDevice.Process(pdfPage, textStream);

            //            //close memory stream
            //            textStream.Close();

            //            //get text from memory stream
            //            var extractedText = Encoding.Unicode.GetString(textStream.ToArray());
            //            if (string.IsNullOrWhiteSpace(extractedText))
            //            {
            //                continue;
            //            }
            //            var lineBreakTexts = extractedText.Split(new[] { "\r\n" , "." }, StringSplitOptions.RemoveEmptyEntries)
            //                .GroupBy(m => m)
            //                .Where(m => m.Count() == 1 && m.Key != string.Empty)
            //                .Select(s => s.Key);

            //            foreach (var lineBreakText in lineBreakTexts)
            //            {
            //                var txt = StripUnwantedChars(lineBreakText);

            //                if (pageTexts.Contains(txt))
            //                {
            //                    continue;
            //                }
            //                var d = LevenshteinDistance.Compute(pageTexts.LastOrDefault(), txt);
            //                if (d < 5)
            //                {
            //                    TraceLog.WriteInfo(
            //                        $"LevenshteinDistance less then 10 between {pageTexts.LastOrDefault()} and  {extractedText}");
            //                    continue;
            //                }
            //                pageTexts.Add(txt);
            //            }


            //        }
            //        //builder.Append(extractedText);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    TraceLog.WriteError("trying to extract pdf text", ex);
            //}
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

    //static class LevenshteinDistance
    //{
    //    /// <summary>
    //    /// Computes the Damerau-Levenshtein Distance between two strings, represented as arrays of
    //    /// integers, where each integer represents the code point of a character in the source string.
    //    /// Includes an optional threshhold which can be used to indicate the maximum allowable distance.
    //    /// </summary>
    //    /// <param name="source">An array of the code points of the first string</param>
    //    /// <param name="target">An array of the code points of the second string</param>
    //    /// <param name="threshold">Maximum allowable distance</param>
    //    /// <returns>Int.MaxValue if threshhold exceeded; otherwise the Damerau-Leveshteim distance between the strings</returns>
    //    public static int DamerauLevenshteinDistance(string source, string target, int threshold)
    //    {

    //        int length1 = source.Length;
    //        int length2 = target.Length;

    //        // Return trivial case - difference in string lengths exceeds threshhold
    //        if (Math.Abs(length1 - length2) > threshold) { return int.MaxValue; }

    //        // Ensure arrays [i] / length1 use shorter length 
    //        if (length1 > length2)
    //        {
    //            Swap(ref target, ref source);
    //            Swap(ref length1, ref length2);
    //        }

    //        int maxi = length1;
    //        int maxj = length2;

    //        int[] dCurrent = new int[maxi + 1];
    //        int[] dMinus1 = new int[maxi + 1];
    //        int[] dMinus2 = new int[maxi + 1];
    //        int[] dSwap;

    //        for (int i = 0; i <= maxi; i++) { dCurrent[i] = i; }

    //        int jm1 = 0, im1 = 0, im2 = -1;

    //        for (int j = 1; j <= maxj; j++)
    //        {

    //            // Rotate
    //            dSwap = dMinus2;
    //            dMinus2 = dMinus1;
    //            dMinus1 = dCurrent;
    //            dCurrent = dSwap;

    //            // Initialize
    //            int minDistance = int.MaxValue;
    //            dCurrent[0] = j;
    //            im1 = 0;
    //            im2 = -1;

    //            for (int i = 1; i <= maxi; i++)
    //            {

    //                int cost = source[im1] == target[jm1] ? 0 : 1;

    //                int del = dCurrent[im1] + 1;
    //                int ins = dMinus1[i] + 1;
    //                int sub = dMinus1[im1] + cost;

    //                //Fastest execution for min value of 3 integers
    //                int min = (del > ins) ? (ins > sub ? sub : ins) : (del > sub ? sub : del);

    //                if (i > 1 && j > 1 && source[im2] == target[jm1] && source[im1] == target[j - 2])
    //                    min = Math.Min(min, dMinus2[im2] + cost);

    //                dCurrent[i] = min;
    //                if (min < minDistance) { minDistance = min; }
    //                im1++;
    //                im2++;
    //            }
    //            jm1++;
    //            if (minDistance > threshold) { return int.MaxValue; }
    //        }

    //        int result = dCurrent[maxi];
    //        return (result > threshold) ? int.MaxValue : result;
    //    }
    //    static void Swap<T>(ref T arg1, ref T arg2)
    //    {
    //        T temp = arg1;
    //        arg1 = arg2;
    //        arg2 = temp;
    //    }
    //    public static int Compute(string s, string t)
    //    {
    //        if (string.IsNullOrEmpty(s))
    //        {
    //            if (string.IsNullOrEmpty(t))
    //                return 0;
    //            return t.Length;
    //        }

    //        if (string.IsNullOrEmpty(t))
    //        {
    //            return s.Length;
    //        }

    //        int n = s.Length;
    //        int m = t.Length;
    //        int[,] d = new int[n + 1, m + 1];

    //        // initialize the top and right of the table to 0, 1, 2, ...
    //        for (int i = 0; i <= n; d[i, 0] = i++) ;
    //        for (int j = 1; j <= m; d[0, j] = j++) ;

    //        for (int i = 1; i <= n; i++)
    //        {
    //            for (int j = 1; j <= m; j++)
    //            {
    //                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
    //                int min1 = d[i - 1, j] + 1;
    //                int min2 = d[i, j - 1] + 1;
    //                int min3 = d[i - 1, j - 1] + cost;
    //                d[i, j] = Math.Min(Math.Min(min1, min2), min3);
    //            }
    //        }
    //        return d[n, m];
    //    }
    //}
}
