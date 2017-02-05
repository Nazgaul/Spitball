using System.Threading;
using Aspose.Words;
using Aspose.Words.Saving;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class WordProcessor : DocumentProcessor
    {
        private const string CacheVersion = CacheVersionPrefix + "6";

        public WordProcessor(IBlobProvider blobProvider, IBlobProvider2<IPreviewContainer> blobProviderPreview, IBlobProvider2<ICacheContainer> blobProviderCache)
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
            var blobName = GetBlobNameFromUri(blobUri);

            var word = new AsyncLazy<Document>(async () =>
            {
                SetLicense();
                using (var sr = await BlobProvider.DownloadFileAsync(blobUri, cancelToken))
                {
                    return new Document(sr);
                }
            });

            var svgOptions = new SvgSaveOptions { ShowPageBorder = false, FitToViewPort = true, JpegQuality = 85, ExportEmbeddedImages = true, PageCount = 1 };
            var retVal = await UploadPreviewCacheToAzureAsync(blobUri, indexNum,
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

            return new PreviewResult { Content = retVal, ViewName = "Svg" };
        }
        protected string CreateCacheFileName(string blobName, int index)
        {
            return string.Format("{0}{3}_{2}_{1}.svg", Path.GetFileNameWithoutExtension(blobName), Path.GetExtension(blobName), index, CacheVersion);
        }

        public static readonly string[] WordExtensions = { ".rtf", ".docx", ".doc", ".odt" };
        public override bool CanProcessFile(Uri blobName)
        {
            return blobName.AbsoluteUri.StartsWith(BlobProvider.StorageContainerUrl) && WordExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        }

        public override async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri,
            CancellationToken cancelToken = default(CancellationToken))
        {
            try
            {
                var path = await BlobProvider.DownloadToLocalDiskAsync(blobUri, cancelToken);
                SetLicense();
                var word = new Document(path);

                return await ProcessFileAsync(blobUri, () =>
                {
                    var imgOptions = new ImageSaveOptions(SaveFormat.Jpeg)
                    {
                        JpegQuality = 80,
                        Resolution = 150
                    };

                    var ms = new MemoryStream();
                    word.Save(ms, imgOptions);
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms;

                }, () => word.PageCount, CacheVersion, cancelToken);

            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile word", ex);
                return null;
            }
        }

        private string ExtractDocumentText(Document doc)
        {
            try
            {
                //int sectionInt = 0;
                //var sb = new StringBuilder();
                //do
                //{
                //    var section = doc.Sections[sectionInt];
                //    sb.AppendLine(section.ToString(SaveFormat.Text));
                //    sectionInt++;
                //}while (sb.Length < 5000);

               
                var str = doc.ToString(SaveFormat.Text);
                str = StripUnwantedChars(str);
                return str;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Failed to extract text from doc", ex);
                return string.Empty;
            }
        }



        public override async Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            //var blobName = GetBlobNameFromUri(blobUri);
           // var path = await BlobProvider.DownloadToLocalDiskAsync(blobUri, cancelToken);
            SetLicense();
            using (var stream = await BlobProvider.OpenBlobStreamAsync(blobUri, cancelToken))
            {
                var word = new Document(stream);
                return ExtractDocumentText(word);
            }
            
        }

    }
}
