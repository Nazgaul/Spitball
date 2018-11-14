using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Saving;

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


        public async Task ProcessFilesAsync(Stream stream,
            Func<Stream, string, Task> pagePreviewCallback,
            Func<string, int, Task> metaCallback,
            CancellationToken token)
        {
            var word = new Document(stream);
            var txt = ExtractDocumentText(word);

            await metaCallback(txt, word.PageCount);

            var t = new List<Task>();
            var svgOptions = new SvgSaveOptions
            {
                ShowPageBorder = false,
                FitToViewPort = true,
                JpegQuality = 85,
                ExportEmbeddedImages = true,
                PageCount = 1
            };
            for (var i = 0; i < word.PageCount; i++)
            {
                var ms = new MemoryStream();
                word.Save(ms, svgOptions);
                ms.Seek(0, SeekOrigin.Begin);
                t.Add(pagePreviewCallback(ms, $"{i}.svg").ContinueWith(_=> ms.Dispose(), token));
            }
            await Task.WhenAll(t);

        }
    }
}
