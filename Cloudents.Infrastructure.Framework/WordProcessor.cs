using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Saving;
using Cloudents.Core;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Framework
{
    public class WordProcessor : Processor, IPreviewProvider
    {
        private const string CacheVersion = CacheVersionPrefix + "6";

        public WordProcessor(
                Uri blobUri,
                IBlobProvider blobProvider,
                IBlobProvider<CacheContainer> blobProviderCache)
            //: base(blobProvider, blobProviderPreview, blobProviderCache)
            : base(blobProvider, blobProviderCache, blobUri)
        {
            SetLicense();
        }

        private static void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }

        public Task<IEnumerable<string>> ConvertFileToWebsitePreviewAsync(
            int indexNum,
            CancellationToken cancelToken)
        {
            var blobName = BlobProvider.GetBlobNameFromUri(BlobUri);

            var word = new AsyncLazy<Document>(async () =>
            {
                SetLicense();
                using (var sr = await BlobProvider.DownloadFileAsync(BlobUri, cancelToken).ConfigureAwait(false))
                {
                    return new Document(sr);
                }
            });

            var svgOptions = new SvgSaveOptions { ShowPageBorder = false, FitToViewPort = true, JpegQuality = 85, ExportEmbeddedImages = true, PageCount = 1 };
            return UploadPreviewCacheToAzureAsync(indexNum,
                i => CreateCacheFileName(blobName, i),
                async z =>
                {
                    svgOptions.PageIndex = z;
                    var ms = new MemoryStream();
                    var w = await word;
                    w.Save(ms, svgOptions);
                    return ms;
                }, CacheVersion, "image/svg+xml", cancelToken
            );
        }

        protected static string CreateCacheFileName(string blobName, int index)
        {
            return
                $"{Path.GetFileNameWithoutExtension(blobName)}{CacheVersion}_{index}_{Path.GetExtension(blobName)}.svg";
        }

        public static readonly string[] WordExtensions = { ".rtf", ".docx", ".doc", ".odt" };
        //public static bool CanProcessFile(Uri blobName)
        //{
        //    return WordExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        //}

        //public override async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri,
        //    CancellationToken cancelToken = default(CancellationToken))
        //{
        //    try
        //    {
        //        var path = await BlobProvider.DownloadToLocalDiskAsync(blobUri, cancelToken).ConfigureAwait(false);
        //        SetLicense();
        //        var word = new Document(path);

        //        return await ProcessFileAsync(blobUri, () =>
        //        {
        //            var imgOptions = new ImageSaveOptions(SaveFormat.Jpeg)
        //            {
        //                JpegQuality = 80,
        //                Resolution = 150
        //            };

        //            var ms = new MemoryStream();
        //            word.Save(ms, imgOptions);
        //            ms.Seek(0, SeekOrigin.Begin);
        //            return ms;
        //        }, () => word.PageCount, CacheVersion, cancelToken).ConfigureAwait(false);
        //    }
        //    catch (UnsupportedFileFormatException)
        //    {
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        m_Logger.Exception(ex);
        //        return null;
        //    }
        //}

        //private string ExtractDocumentText(Document doc)
        //{
        //    try
        //    {
        //        var str = doc.ToString(SaveFormat.Text);
        //        return StripUnwantedChars(str);
        //    }
        //    catch (Exception ex)
        //    {
        //        m_Logger.Exception(ex);
        //        return string.Empty;
        //    }
        //}

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
    }
}
