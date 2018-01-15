using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Framework
{
    public class ImageProcessor : IPreviewProvider
    {
        private readonly Uri _blobUri;
        private readonly IBlobProvider _blobProvider;


        public ImageProcessor(Uri blobUri, IBlobProvider blobProvider)
        {
            _blobUri = blobUri;
            _blobProvider = blobProvider;
        }

        public Task<IEnumerable<string>> ConvertFileToWebsitePreviewAsync(int indexNum,
            CancellationToken cancelToken)
        {
            if (indexNum > 0)
            {
                return Task.FromResult(Enumerable.Empty<string>());
            }
            var blobName = _blobProvider.GetBlobNameFromUri(_blobUri);


            var uriBuilder =
                new UriBuilder("https://az779114.vo.msecnd.net/preview/");
            //this was to support chat - we don't at the time
            //if (!string.IsNullOrEmpty(m_BlobProviderPreview.RelativePath()))
            //{
            //    uriBuilder.Path += m_BlobProviderPreview.RelativePath() + "/";
            //}
            uriBuilder.Path += $"{blobName}.jpg";
            uriBuilder.Query = "width=1024&height=768";
            var blobsNamesInCache = new List<string>
            {
                uriBuilder.ToString()
            };
            return Task.FromResult<IEnumerable<string>>(blobsNamesInCache);
        }

        public static readonly string[] ImageExtensions = { ".jpg", ".gif", ".png", ".jpeg", ".bmp" };

        //public override bool CanProcessFile(Uri blobName)
        //{
        //    return blobName.AbsoluteUri.StartsWith(BlobProvider.StorageContainerUrl)
        //           && ImageExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        //}

        //public override async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri,
        //    CancellationToken cancelToken = default(CancellationToken))
        //{
        //    try
        //    {
        //        var blobName = GetBlobNameFromUri(blobUri);
        //        var previewBlobName = blobName + ".jpg";
        //        if (await m_BlobProviderPreview.ExistsAsync(previewBlobName, cancelToken).ConfigureAwait(false))
        //        {
        //            return null;
        //        }
        //        using (var stream = await BlobProvider.DownloadFileAsync(blobUri, cancelToken).ConfigureAwait(false))
        //        {
        //            if (stream.Length == 0)
        //            {
        //                TraceLog.WriteError("image is empty" + blobUri);
        //            }

        //            using (var ms = new MemoryStream())
        //            {
        //                var settings2 = new ResizeSettings
        //                {
        //                    Format = "jpg"
        //                };
        //                ImageBuilder.Current.Build(stream, ms, settings2, false);

        //                await m_BlobProviderPreview.UploadStreamAsync(previewBlobName, ms, "image/jpeg", cancelToken).ConfigureAwait(false);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("PreProcessFile image blobUri: " + blobUri, ex);
        //    }
        //    return null;
        //}

        //public override Task<string> ExtractContentAsync(Uri blobUri,
        //    CancellationToken cancelToken = default(CancellationToken))
        //{
        //    return Task.FromResult<string>(null);
        //}
    }
}
