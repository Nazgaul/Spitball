using Aspose.Words;
using Aspose.Words.Saving;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class HtmlProcessor : FileProcessor
    {
        private static readonly string ContentFormat = "<iframe class=\"iframeContent\" src=\"{0}\"></iframe>";

        public HtmlProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {

        }

        private void SetLicense()
        {
            var license = new Aspose.Words.License();
            license.SetLicense("Aspose.Total.lic");
        }
        //using aspose word to create thumbnail
        public override Task<PreProcessFileResult> PreProcessFile(Uri blobUri)
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                Document word = null;

                CancellationTokenSource canceller = new CancellationTokenSource();

                using (var sr = m_BlobProvider.DownloadFile(blobName))
                {
                    SetLicense();
                    word = new Document(sr);
                }
                ImageSaveOptions imgOptions = new ImageSaveOptions(SaveFormat.Jpeg);
                imgOptions.JpegQuality = 100;

                ResizeSettings settings = new ResizeSettings();
                settings.Width = ThumbnailWidth;
                settings.Height = ThumbnailHeight;
                settings.Quality = 80;
                settings.Format = "jpg";

                using (var ms = new MemoryStream())
                {
                    word.Save(ms, imgOptions);
                    ms.Seek(0, SeekOrigin.Begin);
                    using (var output = new MemoryStream())
                    {
                        ImageResizer.ImageBuilder.Current.Build(ms, output, settings);
                        var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV3.jpg";
                        m_BlobProvider.UploadFileThumbnail(thumbnailBlobAddressUri, output, "image/jpeg");
                        return Task.FromResult<PreProcessFileResult>(new PreProcessFileResult { ThumbnailName = thumbnailBlobAddressUri });
                    }
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile html", ex);
                return Task.FromResult<PreProcessFileResult>(new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() });
            }
        }

        public override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum)
        {
            var publicUrl = m_BlobProvider.GenerateSharedAccressReadPermissionInStorage(blobUri, 20);
            return Task.FromResult<PreviewResult>(new PreviewResult(String.Format(ContentFormat, publicUrl)));

        }
        public static readonly string[] htmlExtensions = { ".htm", ".html" };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(m_BlobProvider.BlobContainerUrl))
            {
                return htmlExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;
        }

        public override string GetDefaultThumbnailPicture()
        {
            return Zbang.Zbox.Infrastructure.Thumbnail.ThumbnailProvider.DefaultFileTypePicture;
        }
    }
}
