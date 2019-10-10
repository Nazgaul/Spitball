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
    public class PdfProcessor : IPreviewProvider, IDisposable //: Processor, IPreviewProvider
    {

        public PdfProcessor()
        {
            using (var sr = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cloudents.Infrastructure.Framework.Aspose.Total.lic"))
            {
                var license = new License();
                license.SetLicense(sr);
            }
        }

        private Lazy<Document> _doc;

        public void Init(Func<Stream> stream)
        {
            _doc = new Lazy<Document>(() => new Document(stream()));
        }

        public void Init(Func<string> stream)
        {
            _doc = new Lazy<Document>(() => new Document(stream()));
        }

        public (string text, int pagesCount) ExtractMetaContent()
        {
            var t = _doc.Value;
            var txt = ExtractPdfText(t);
            return (txt , t.Pages.Count);
        }

        public async Task ProcessFilesAsync(IEnumerable<int> previewDelta, Func<Stream, string, Task> pagePreviewCallback,
            CancellationToken token)
        {
            var resolution = new Resolution(150);
            var jpegDevice = new JpegDevice(resolution, 90);
            var t = new List<Task>();
            var doc = _doc.Value;
            var diff = Enumerable.Range(0, doc.Pages.Count).Except(previewDelta);
            foreach (int item in diff)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
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


        public void Dispose()
        {
            if (_doc != null && _doc.IsValueCreated)
            {
                _doc?.Value?.Dispose();
            }
        }
       
    }
}
