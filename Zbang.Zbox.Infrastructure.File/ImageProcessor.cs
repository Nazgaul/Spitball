using System.Threading;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class ImageProcessor : FileProcessor, IProfileProcessor
    {
        private readonly IBlobProvider2<IPreviewContainer> m_BlobProviderPreview;

        public ImageProcessor() : base(null)
        {

        }
        public ImageProcessor(IBlobProvider blobProvider, IBlobProvider2<IPreviewContainer> blobProviderPreview)
            : base(blobProvider)
        {
            m_BlobProviderPreview = blobProviderPreview;
        }

        public override Task<PreviewResult> ConvertFileToWebsitePreviewAsync(Uri blobUri, int indexNum,
            CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = GetBlobNameFromUri(blobUri);
            if (indexNum > 0)
            {
                return Task.FromResult(new PreviewResult { Content = new List<string>() });
            }

            var uriBuilder =
                new UriBuilder("https://az779114.vo.msecnd.net/preview/");
            if (!string.IsNullOrEmpty(m_BlobProviderPreview.RelativePath()))
            {
                uriBuilder.Path += m_BlobProviderPreview.RelativePath() + "/";
            }
            uriBuilder.Path += $"{blobName}.jpg";
            uriBuilder.Query = "width=1024&height=768";
            var blobsNamesInCache = new List<string>
            {
                uriBuilder.ToString()
            };
            return Task.FromResult(new PreviewResult { ViewName = "Image", Content = blobsNamesInCache });
        }

        public async Task<Stream> ProcessFileAsync(Stream stream, int width, int height)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid subscription key.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "b044cd956e0f4af481cdc7e4c5077aeb");

            // Request parameters and URI.
            string requestParameters = "width=200&height=150&smartCropping=true";
            string uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/generateThumbnail?" + requestParameters;

            // Request body. Try this sample with a locally stored JPEG image.
            byte[] byteData = stream.ConvertToByteArray(); // GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync(uri, content).ConfigureAwait(false);
                return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
            //stream.Seek(0, SeekOrigin.Begin);

            //var settings = new ResizeSettings
            //{

            //    Mode = FitMode.Crop,
            //    Width = width,
            //    Height = height,
            //    Quality = 80,
            //    Format = "jpg"
            //};

            //var ms = new MemoryStream();
            //ImageBuilder.Current.Build(stream, ms, settings);
            //return ms;
        }


        public static readonly string[] ImageExtensions = { ".jpg", ".gif", ".png", ".jpeg", ".bmp" };

        public override bool CanProcessFile(Uri blobName)
        {
            return blobName.AbsoluteUri.StartsWith(BlobProvider.StorageContainerUrl) &&
                   ImageExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        }

        public override async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri,
            CancellationToken cancelToken = default(CancellationToken))
        {
            try
            {
                var blobName = GetBlobNameFromUri(blobUri);
                var previewBlobName = blobName + ".jpg";
                if (await m_BlobProviderPreview.ExistsAsync(previewBlobName))
                {
                    return null;
                }
                using (var stream = await BlobProvider.DownloadFileAsync(blobUri, cancelToken))
                {
                    if (stream.Length == 0)
                    {
                        TraceLog.WriteError("image is empty" + blobUri);
                    }


                    using (var ms = new MemoryStream())
                    {
                        var settings2 = new ResizeSettings
                        {
                            Format = "jpg"
                        };
                        ImageBuilder.Current.Build(stream, ms, settings2, false);
                        
                        await m_BlobProviderPreview.UploadStreamAsync(previewBlobName, ms, "image/jpeg", cancelToken);
                    }

                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PreProcessFile image blobUri: " + blobUri, ex);

            }
            return null;


        }



        public override Task<string> ExtractContentAsync(Uri blobUri,
            CancellationToken cancelToken = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }


    }
}
