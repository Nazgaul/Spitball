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
    public class PdfProcessor : IPreviewProvider2, IDisposable //: Processor, IPreviewProvider
    {

        public PdfProcessor()
        {
            using (var sr = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cloudents.Infrastructure.Framework.Aspose.Total.lic"))
            {
                var license = new License();
                license.SetLicense(sr);
            }
        }

        private Document _doc;

        public void Init(Stream stream)
        {
            _doc = new Document(stream);
        }

        public (string text, int pagesCount) ExtractMetaContent()
        {
            var txt = ExtractPdfText(_doc);
            return (txt , _doc.Pages.Count - 1);
        }

        public async Task ProcessFilesAsync(IEnumerable<int> previewDelta, Func<Stream, string, Task> pagePreviewCallback,
            CancellationToken token)
        {
            var resolution = new Resolution(150);
            var jpegDevice = new JpegDevice(resolution, 90);
            var t = new List<Task>();

            var diff = Enumerable.Range(0, _doc.Pages.Count - 1);
            diff = diff.Except(previewDelta);
            foreach (int item in diff)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                var page = _doc.Pages[item + 1];
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

        public void Dispose()
        {
            _doc?.Dispose();
        }
    }
}
