using System.Threading;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class ImageProcessor : FileProcessor , IProfileProcessor
    {

        public ImageProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {

        }
        public override Task<PreviewResult> ConvertFileToWebSitePreviewAsync(Uri blobUri, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = GetBlobNameFromUri(blobUri);
            if (indexNum > 0)
            {
                return Task.FromResult(new PreviewResult { Content = new List<string>() });
            }
            var blobsNamesInCache = new List<string>
            {
                $"https://az779114.vo.msecnd.net/preview/{blobName}.jpg?width={1024}&height={768}"
            };
            return Task.FromResult(new PreviewResult { ViewName = "Image", Content = blobsNamesInCache });
        }

        public Stream ProcessFile(Stream stream, int width, int height)
        {
            stream.Seek(0, SeekOrigin.Begin);

            var settings = new ResizeSettings
            {

                //Anchor = ContentAlignment.MiddleCenter,
                //BackgroundColor = Color.White,
                Mode = FitMode.Crop,
                //Scale = ScaleMode.UpscaleCanvas,
                Width = width,
                Height = height,
                Quality = 80,
                Format = "jpg"
            };

            var ms = new MemoryStream();
            ImageBuilder.Current.Build(stream, ms, settings);
            return ms;
        }


        public static readonly string[] ImageExtensions = { ".jpg", ".gif", ".png", ".jpeg", ".bmp" };

        public override bool CanProcessFile(Uri blobName)
        {
            return blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl) && ImageExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        }

        public override async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                using (var stream = await BlobProvider.DownloadFileAsync(blobName, cancelToken))
                {
                    if (stream.Length == 0)
                    {
                        TraceLog.WriteError("image is empty" + blobName);
                        return null;
                    }
                   

                    using (var ms = new MemoryStream())
                    {
                        var settings2 = new ResizeSettings
                        {
                            Format = "jpg"
                        };
                        ImageBuilder.Current.Build(stream, ms, settings2, false);
                        await BlobProvider.UploadFilePreviewAsync(blobName + ".jpg", ms, "image/jpeg", cancelToken);
                    }
                    //using (var outPutStream = ProcessFile(stream, ThumbnailWidth, ThumbnailHeight))
                    //{
                    //    //var thumbnailBlobAddressUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV4.jpg";
                    //    //await BlobProvider.UploadFileThumbnailAsync(thumbnailBlobAddressUri, outPutStream, "image/jpeg", cancelToken);
                    //    //return new PreProcessFileResult { ThumbnailName = thumbnailBlobAddressUri };
                    //}

                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile image blobUri: " + blobUri, ex);
                
            }
            return null;

        }

        //public override string GetDefaultThumbnailPicture()
        //{
        //    return DefaultPicture.ImageFileTypePicture;
        //}

        public override Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }
    }
}
