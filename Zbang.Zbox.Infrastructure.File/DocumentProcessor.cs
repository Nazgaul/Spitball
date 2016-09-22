using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public abstract class DocumentProcessor : FileProcessor
    {
        protected const int NumberOfFilesInGroup = 15;
        protected const string DatePattern = "M-d-yy";

        protected readonly IBlobProvider2<IPreviewContainer> BlobProviderPreview;
        protected readonly IBlobProvider2<ICacheContainer> BlobProviderCache;


        protected DocumentProcessor(
            IBlobProvider blobProvider,
            IBlobProvider2<IPreviewContainer> blobProviderPreview,
            IBlobProvider2<ICacheContainer> blobProviderCache)
            : base(blobProvider)
        {
            BlobProviderPreview = blobProviderPreview;
            BlobProviderCache = blobProviderCache;
        }

        protected async Task<PreProcessFileResult> ProcessFileAsync(Uri blobUri,
            Func<Stream> previewStream,
            Func<int> getPageCount,
            string getCacheVersionPrefix,
            CancellationToken token)
        {
            var t2 = UploadMetaDataAsync(blobUri, getPageCount(), getCacheVersionPrefix, token);
            var blobName = GetBlobNameFromUri(blobUri) + ".jpg";
            if (!await BlobProviderPreview.ExistsAsync(blobName))
            {
                using (var msPreview = previewStream())
                {

                    var t3 = BlobProviderPreview.UploadStreamAsync(blobName, msPreview, "image/jpeg", token);
                    await Task.WhenAll(t2, t3);
                    return null;
                }
            }
            await t2;
            return null;

        }

        protected async Task<IEnumerable<string>> UploadPreviewCacheToAzureAsync(Uri blobName,
            int startPage,
            Func<int, string> pageCacheBlobName,
            Func<int, Task<Stream>> convertPageToPreview, string cacheVersion,
            string mimeType, CancellationToken token
            )
        {
            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task>();

            var meta = await BlobProvider.FetchBlobMetaDataAsync(blobName, token);
            meta = RemoveOldMetaTags(meta, cacheVersion);
            string sPageCount;
            var pageCount = int.MaxValue;
            if (meta.TryGetValue(PagesInDocsMetaKey, out sPageCount))
            {
                int.TryParse(sPageCount, out pageCount);
            }
            for (var pageIndex = startPage; pageIndex < Math.Min(startPage + NumberOfFilesInGroup, pageCount); pageIndex++)
            {
                var metaDataKey = cacheVersion + pageIndex;
                var cacheblobName = pageCacheBlobName(pageIndex);
                if (GetPrviewImage(pageIndex, metaDataKey, ref meta, ref blobsNamesInCache, cacheblobName))
                {
                    continue;
                }
                try
                {
                    using (var ms = await convertPageToPreview(pageIndex))
                    {
                        var compressor = new Compress();
                        var sr = compressor.CompressToGzip(ms);
                        parallelTask.Add(BlobProviderCache.UploadByteArrayAsync(cacheblobName, sr, mimeType, true, 30));
                        blobsNamesInCache.Add(BlobProviderCache.GenerateSharedAccressReadPermission(cacheblobName, 30));
                        meta[metaDataKey] = DateTime.UtcNow.ToString(DatePattern);
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
            var t = BlobProvider.SaveMetaDataToBlobAsync(blobName, meta, token);
            var tasks = new List<Task>();
            tasks.AddRange(parallelTask);
            tasks.Add(t);
            await Task.WhenAll(tasks);
            return blobsNamesInCache;
        }

        private bool GetPrviewImage(int pageIndex, string metaDataKey,
            ref IDictionary<string, string> meta,
            ref List<string> blobsNamesInCache,
            string cacheblobName)
        {
            if (pageIndex > NumberOfFilesInGroup)
            {
                if (BlobProviderCache.Exists(cacheblobName))
                {
                    blobsNamesInCache.Add(BlobProviderCache.GenerateSharedAccressReadPermission(cacheblobName, 20));
                    //blobsNamesInCache.Add(BlobProvider.GenerateSharedAccressReadPermissionInCacheWithoutMeta(cacheblobName, 20));
                    return true;
                }
            }
            else
            {
                string value;
                if (meta.TryGetValue(metaDataKey, out value))
                {
                    blobsNamesInCache.Add(BlobProviderCache.GenerateSharedAccressReadPermission(cacheblobName, 20));
                    //blobsNamesInCache.Add(BlobProvider.GenerateSharedAccressReadPermissionInCacheWithoutMeta(cacheblobName, 20));
                    meta[metaDataKey] = DateTime.UtcNow.ToString(DatePattern);
                    return true;
                }
            }
            return false;
        }
    }
}
