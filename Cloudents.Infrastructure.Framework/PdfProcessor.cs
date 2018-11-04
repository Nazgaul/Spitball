using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Aspose.Pdf.Text;
using Aspose.Pdf.Text.TextOptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Framework
{
    public class PdfProcessor : IPreviewProvider2 //: Processor, IPreviewProvider
    {
        //private const string CacheVersion = CacheVersionPrefix + "4";

        public PdfProcessor()

        //IBlobProvider<OldSbFilesContainerName> blobProvider,
        //IBlobProvider<OldCacheContainer> blobProviderCache)
        //: base(blobProvider,  blobProviderCache, blobUri)
        {
            SetLicense();
        }


        public async Task CreatePreviewFilesAsync(MemoryStream stream,
            Func<Stream, string, Task> callback,
            Func<string, Task> textCallback,
            CancellationToken token)
        {
            var pdf = new Document(stream);
            var txt = ExtractPdfText(pdf);

            await textCallback(txt);
            var resolution = new Resolution(150);
            var jpegDevice = new JpegDevice(resolution, 90);

            var t = new List<Task>();
            for (int j = 1; j < pdf.Pages.Count; j++)
            {
                var page = pdf.Pages[j];
                var ms = new MemoryStream();
                jpegDevice.Process(page, ms);
                t.Add(callback(ms, $"{j - 1}.jpg").ContinueWith(_ => ms.Dispose(), token));

            }
            //foreach (Page page in pdf.Pages)
            //{
            //    //using (var ms = new MemoryStream())
            //    //{
            //    //    jpegDevice.Process(page,ms);
            //    //    t.Add(callback(ms,$"{i}.pdf"));
            //    //}

            //    i++;
            //}

            await Task.WhenAll(t);

        }

        private static void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }


        private static string ExtractPdfText(Document doc)
        {
            var textAbsorber = new TextAbsorber
            {
                ExtractionOptions = new TextExtractionOptions(TextExtractionOptions.TextFormattingMode.Pure)
            };
            for (var i = 1; i <= Math.Min(doc.Pages.Count, 20); i++)
            {
                try
                {
                    doc.Pages[i].Accept(textAbsorber);
                }
                catch (ArgumentException)
                {
                    continue;
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }
            return textAbsorber.Text;

        }

        //public async Task<IEnumerable<string>> ConvertFileToWebsitePreviewAsync(int indexNum, CancellationToken cancelToken = default(CancellationToken))
        //{
        //    //var blobName = BlobProvider.GetBlobNameFromUri(BlobUri);

        //    var resolution = new Resolution(150);
        //    var jpegDevice = new JpegDevice(resolution, 90);
        //    Stream blobSr = null;

        //    var pdf = new AsyncLazy<Document>(async () =>
        //    {
        //        SetLicense();
        //        blobSr = await BlobProvider.DownloadFileAsync(BlobUri, cancelToken).ConfigureAwait(false);
        //        return new Document(blobSr);
        //    });

        //    var retVal = await UploadPreviewCacheToAzureAsync(
        //        indexNum,
        //        i => CreateCacheFileName(BlobUri, i),
        //        async z =>
        //        {
        //            var ms = new MemoryStream();
        //            var p = await pdf;
        //            jpegDevice.Process(p.Pages[z + 1], ms);
        //            return ms;
        //        }, CacheVersion, "image/jpg", cancelToken).ConfigureAwait(false);

        //    if (pdf.Instance.IsValueCreated && blobSr != null)
        //    {
        //        blobSr.Dispose();
        //        pdf.Instance.Value.Dispose();
        //    }
        //    return retVal;
        //}

        //protected static string CreateCacheFileName(string blobName, int index)
        //{
        //    return
        //        $"{Path.GetFileNameWithoutExtension(blobName)}{CacheVersion}_{index}_{Path.GetExtension(blobName)}.jpg";
        //}

        public static readonly string[] PdfExtensions = { ".pdf" };



    }
}
