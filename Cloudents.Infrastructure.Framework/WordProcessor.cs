using Aspose.Words;
using Aspose.Words.Saving;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Framework
{
    public class WordProcessor : IPreviewProvider2
    {
        public WordProcessor()
        {
            using (var sr = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cloudents.Infrastructure.Framework.Aspose.Total.lic"))
            {
                var license = new License();
                license.SetLicense(sr);
            }
        }

        public static readonly string[] Extensions = { ".rtf", ".docx", ".doc", ".odt" };


        private static string ExtractDocumentText(Document doc)
        {
            try
            {
                return doc.ToString(SaveFormat.Text);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private Lazy<Document> _word;

        public void Init(Stream stream)
        {
            _word = new Lazy<Document>(()=> new Document(stream));
        }
        public (string text, int pagesCount) ExtractMetaContent()
        {
            var t = _word.Value;
            var txt = ExtractDocumentText(t);
            return (txt, t.PageCount);
        }

       

        public async Task ProcessFilesAsync(IEnumerable<int> previewDelta, Func<Stream, string, Task> pagePreviewCallback,
            CancellationToken token)
        {
            
            var t = new List<Task>();
            var svgOptions = new ImageSaveOptions(SaveFormat.Jpeg)
            {
                JpegQuality = 90,
                Scale = 1.2F,
                PageCount = 1
            };

            var diff = Enumerable.Range(0, _word.Value.PageCount);
            diff = diff.Except(previewDelta);
            foreach (var item in diff)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                var ms = new MemoryStream();
                _word.Value.Save(ms, svgOptions);
                ms.Seek(0, SeekOrigin.Begin);
                t.Add(pagePreviewCallback(ms, $"{item}.jpg").ContinueWith(_ => ms.Dispose(), token));
            }

            await Task.WhenAll(t);

        }
    }
}
