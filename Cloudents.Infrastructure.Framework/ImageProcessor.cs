using System.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageResizer;

namespace Cloudents.Infrastructure.Framework
{
    public class ImageProcessor : IPreviewProvider2
    {
        public ImageProcessor()
        {
        }


        //public Task<IEnumerable<string>> ConvertFileToWebsitePreviewAsync(int indexNum,
        //    CancellationToken cancelToken)
        //{
        //    if (indexNum > 0)
        //    {
        //        return Task.FromResult(Enumerable.Empty<string>());
        //    }
        //    var blobName = _blobProvider.GetBlobNameFromUri(_blobUri);


        //    var uriBuilder =
        //        new UriBuilder("https://az779114.vo.msecnd.net/preview/");
        //    this was to support chat -we don't at the time
        //    if (!string.IsNullOrEmpty(m_BlobProviderPreview.RelativePath()))
        //    {
        //        uriBuilder.Path += m_BlobProviderPreview.RelativePath() + "/";
        //    }
        //    uriBuilder.Path += $"{_blobUri}.jpg";
        //    uriBuilder.Query = "width=1024&height=768";
        //    var blobsNamesInCache = new List<string>
        //    {
        //        uriBuilder.ToString()
        //    };
        //    return Task.FromResult<IEnumerable<string>>(blobsNamesInCache);
        //}

        public static readonly string[] ImageExtensions = { ".jpg", ".gif", ".png", ".jpeg", ".bmp" };

       

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

       
       

        public async Task ProcessFilesAsync(MemoryStream stream, 
            Func<Stream, string, Task> pagePreviewCallback,
            Func<string, Task> textCallback, 
            Func<int, Task> pageCountCallback,
            CancellationToken token)
        {
            await pageCountCallback(1);
            using (var ms = new MemoryStream())
            {
                var settings2 = new ResizeSettings
                {
                    Format = "jpg",
                    Width = 1024,
                    Height = 768
                };
                ImageBuilder.Current.Build(stream, ms, settings2, false);


                await pagePreviewCallback(ms, "0.jpg");
            }
        }


    }
}
