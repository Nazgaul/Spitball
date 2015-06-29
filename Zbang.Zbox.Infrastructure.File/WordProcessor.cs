using System.Threading;
using Aspose.Words;
using Aspose.Words.Saving;
using ImageResizer;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class WordProcessor : DocumentProcessor
    {
        const string CacheVersion = CacheVersionPrefix + "6";

        public WordProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {
            SetLicense();
          
        }

        private void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }

        public async override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = GetBlobNameFromUri(blobUri);
            //var indexOfPageGenerate = CalculateTillWhenToDrawPictures(indexNum);

            var word = new AsyncLazy<Document>(async () =>
            {
                SetLicense();
                using (var sr = await BlobProvider.DownloadFileAsync(blobName, cancelToken))
                {
                    return new Document(sr);
                }
            });

            var svgOptions = new SvgSaveOptions { ShowPageBorder = false, FitToViewPort = true, JpegQuality = 85, ExportEmbeddedImages = true, PageCount = 1 };
            var retVal = await UploadPreviewToAzure(blobName, indexNum,
                 i => CreateCacheFileName(blobName, i),
                 async z =>
                 {
                     svgOptions.PageIndex = z;
                     var ms = new MemoryStream();
                     var w = await word;
                     w.Save(ms, svgOptions);
                     return ms;
                 }, CacheVersion, "image/svg+xml"
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
            return blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl) && WordExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        }

        public override async Task<PreProcessFileResult> PreProcessFile(Uri blobUri,
            CancellationToken cancelToken = default(CancellationToken))
        {
            try
            {

                var blobName = GetBlobNameFromUri(blobUri);
                var path = await BlobProvider.DownloadToFileAsync(blobName, cancelToken);
                SetLicense();
                var word = new Document(path);

                return await ProcessFile(blobName, () =>
                {
                    var imgOptions = new ImageSaveOptions(SaveFormat.Jpeg)
                    {
                        JpegQuality = 80,
                    };

                    var ms = new MemoryStream();
                    word.Save(ms, imgOptions);
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms;

                }, () => ExtractDocumentText(word), () => word.PageCount, CacheVersion, () =>
                {
                    var imgOptions = new ImageSaveOptions(SaveFormat.Jpeg)
                    {
                        JpegQuality = 80,
                    };

                    using (var ms = new MemoryStream())
                    {
                        word.Save(ms, imgOptions);
                        ms.Seek(0, SeekOrigin.Begin);
                        var settings = new ResizeSettings
                        {
                            Width = ThumbnailWidth,
                            Height = ThumbnailHeight,
                            Quality = 80,
                            Format = "jpg"
                        };
                        var output = new MemoryStream();
                        ImageBuilder.Current.Build(ms, output, settings);
                        return output;
                    }
                });

            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile word", ex);
                return new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() };
            }
        }

        private string ExtractDocumentText(Document doc)
        {
            try
            {

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

        public override string GetDefaultThumbnailPicture()
        {
            return DefaultPicture.WordFileTypePicture;
        }



        public override async Task<string> ExtractContent(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = GetBlobNameFromUri(blobUri);
            var path = await BlobProvider.DownloadToFileAsync(blobName, cancelToken);
            SetLicense();
            var word = new Document(path);
            return ExtractDocumentText(word);
        }

      
    }
}
