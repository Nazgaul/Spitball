using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Aspose.Pdf.Text;
using Aspose.Pdf.Text.TextOptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Framework
{
    public class PdfProcessor : IPreviewProvider2 //: Processor, IPreviewProvider
    {

        public PdfProcessor()
        {
            using (var sr = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cloudents.Infrastructure.Framework.Aspose.Total.lic"))
            {
                var license = new License();
                license.SetLicense(sr);
            }
        }
        Document doc;

        public void Init(Stream stream)
        {
            doc = new Document(stream);
        }

        public (string text, int pagesCount) ExtractMetaContent()
        {
            var txt = ExtractPdfText(doc);
            return (txt , doc.Pages.Count - 1);
        }

        public int ExtractPagesCount()
        {
            return (doc.Pages.Count - 1);
        }

        public async Task ProcessFilesAsync(List<int> previewDelta, Func<Stream, string, Task> pagePreviewCallback,
            CancellationToken token)
        {
            var resolution = new Resolution(150);
            var jpegDevice = new JpegDevice(resolution, 90);
            var t = new List<Task>();

            var diff = Enumerable.Range(0, doc.Pages.Count - 1);
            diff = diff.Except(previewDelta);
            foreach (int item in diff)
            {
                var page = doc.Pages[item + 1];
                var ms = new MemoryStream();
                jpegDevice.Process(page, ms);
                t.Add(pagePreviewCallback(ms, $"{item}.jpg").ContinueWith(_ => ms.Dispose(), token));
            }

            await Task.WhenAll(t);
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
                catch (IndexOutOfRangeException)
                {

                }
                catch (ArgumentException)
                {
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }
            return textAbsorber.Text;

        }

        public static readonly string[] Extensions = { ".pdf" };

    }
}
