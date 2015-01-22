using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public abstract class DocumentProcessor : FileProcessor
    {
        protected DocumentProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        { }

        protected async Task<PreProcessFileResult> ProcessFile(string blobName, 
            Func<Stream> thumbnailStream,
            Func<String> extractTextFromDocument)
        {
            var thumbnailUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV3.jpg";
            var text = extractTextFromDocument();
            using (var ms = thumbnailStream())
            {
                var t1 = BlobProvider.UploadFileThumbnailAsync(thumbnailUri, ms, "image/jpeg");
                var t2 = UploadMetaData(text, blobName);
                await Task.WhenAll(t1, t2);
            }
            return new PreProcessFileResult
            {
                ThumbnailName = thumbnailUri,
                FileTextContent = text
            };
        }

        protected async Task<IEnumerable<string>> UploadPreviewToAzure(string blobName,
            int startPage,
            int stopPage,
            Func<int, string> createCacheFileName,
            Func<int, string> pageMetaKey,
            Func<int,Task<Stream>> convertPageToPreview
            )
        {
            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task<string>>();
            var tasks = new List<Task>();
            var meta = await BlobProvider.FetechBlobMetaDataAsync(blobName);
            for (var pageIndex = startPage; pageIndex < stopPage; pageIndex++)
            {
                string value;
                var cacheblobName = createCacheFileName(pageIndex);

                var metaDataKey = pageMetaKey(pageIndex);
                if (meta.TryGetValue(metaDataKey, out value))
                {
                    blobsNamesInCache.Add(BlobProvider.GenerateSharedAccressReadPermissionInCacheWithoutMeta(cacheblobName, 20));
                    meta[metaDataKey] = DateTime.UtcNow.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture);
                    continue;
                }
                try
                {
                    using (var ms = await convertPageToPreview(pageIndex))
                    {
                        var compressor = new Compress();
                        var sr = compressor.CompressToGzip(ms);
                        parallelTask.Add(BlobProvider.UploadFileToCacheAsync(cacheblobName, sr, "image/svg+xml", true));
                        meta.Add(metaDataKey, DateTime.UtcNow.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture));
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }

            }
            var t = BlobProvider.SaveMetaDataToBlobAsync(blobName, meta);
            tasks.AddRange(parallelTask);
            tasks.Add(t);
            await Task.WhenAll(tasks);
            blobsNamesInCache.AddRange(parallelTask.Select(s => s.Result));
            return blobsNamesInCache;
        }
    }
}
