using System.Threading;
using Aspose.Words;
using Aspose.Words.Saving;
using ImageResizer;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class HtmlProcessor : FileProcessor
    {
        private const string ContentFormat = "<iframe class=\"iframeContent\" src=\"{0}\"></iframe>";

        public HtmlProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {

        }

        private void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }
        //using aspose word to create thumbnail
        public override Task<PreProcessFileResult> PreProcessFile(Uri blobUri)
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                Document word;

                using (var sr = BlobProvider.DownloadFile(blobName))
                {
                    SetLicense();
                    word = new Document(sr);
                }
                var imgOptions = new ImageSaveOptions(SaveFormat.Jpeg) {JpegQuality = 100};

                var settings = new ResizeSettings
                {
                    Width = ThumbnailWidth,
                    Height = ThumbnailHeight,
                    Quality = 80,
                    Format = "jpg"
                };

                using (var ms = new MemoryStream())
                {
                    word.Save(ms, imgOptions);
                    ms.Seek(0, SeekOrigin.Begin);
                    using (var output = new MemoryStream())
                    {
                        ImageBuilder.Current.Build(ms, output, settings);
                        var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV3.jpg";
                        BlobProvider.UploadFileThumbnail(thumbnailBlobAddressUri, output, "image/jpeg");
                        return Task.FromResult(new PreProcessFileResult { ThumbnailName = thumbnailBlobAddressUri });
                    }
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile html", ex);
                return Task.FromResult(new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() });
            }
        }

        public override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var publicUrl = BlobProvider.GenerateSharedAccressReadPermissionInStorage(blobUri, 20);
            return Task.FromResult(new PreviewResult(String.Format(ContentFormat, publicUrl)));

        }
        public static readonly string[] HtmlExtensions = { ".htm", ".html" };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl))
            {
                return HtmlExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;
        }

        public override string GetDefaultThumbnailPicture()
        {
            return ThumbnailProvider.DefaultFileTypePicture;
        }
    }
}
