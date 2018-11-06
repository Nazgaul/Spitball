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


        //protected static string CreateCacheFileName(string blobName, int index)
        //{
        //    return
        //        $"{Path.GetFileNameWithoutExtension(blobName)}{CacheVersion}_{index}_{Path.GetExtension(blobName)}.svg";
        //}

        public static readonly string[] WordExtensions = { ".rtf", ".docx", ".doc", ".odt" };
        //public static bool CanProcessFile(Uri blobName)
        //{
        //    return WordExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        //}

        

        private static string ExtractDocumentText(Document doc)
        {
            try
            {
                return doc.ToString(SaveFormat.Text);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        //public override async Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        //{
        //    SetLicense();
        //    using (var stream = await BlobProvider.OpenBlobStreamAsync(blobUri, cancelToken).ConfigureAwait(false))
        //    {
        //        try
        //        {
        //            var word = new Document(stream);
        //            return ExtractDocumentText(word);
        //        }
        //        catch (UnsupportedFileFormatException)
        //        {
        //            return null;
        //        }
        //    }
        //}

        public async Task ProcessFilesAsync(Stream stream,
            Func<Stream, string, Task> pagePreviewCallback,
            Func<string, Task> textCallback,
            Func<int, Task> pageCountCallback,
            CancellationToken token)
        {
            var word = new Document(stream);
            var txt = ExtractDocumentText(word);

            await textCallback(txt);
            await pageCountCallback(word.PageCount);

            var t = new List<Task>();
            var imgOptions = new ImageSaveOptions(SaveFormat.Jpeg)
            {
                JpegQuality = 80,
                Resolution = 150
            };
            for (var i = 0; i < word.PageCount; i++)
            {
                var ms = new MemoryStream();
                word.Save(ms, imgOptions);
                ms.Seek(0, SeekOrigin.Begin);
                t.Add(pagePreviewCallback(ms, $"{i}.svg").ContinueWith(_=> ms.Dispose(), token));
            }
            await Task.WhenAll(t);

        }
    }
}
