using System;
using System.Collections.Generic;
using System.IO;
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
            if (previewStream == null) throw new ArgumentNullException(nameof(previewStream));
            if (getPageCount == null) throw new ArgumentNullException(nameof(getPageCount));
            var t2 = UploadMetaDataAsync(blobUri, getPageCount(), getCacheVersionPrefix, token);
            var blobName = GetBlobNameFromUri(blobUri) + ".jpg";
            if (!await BlobProviderPreview.ExistsAsync(blobName).ConfigureAwait(false))
            {
                using (var msPreview = previewStream())
                {
                    if (msPreview == null)
                    {
                        await t2.ConfigureAwait(false);
                        return null;
                    }
                    var t3 = BlobProviderPreview.UploadStreamAsync(blobName, msPreview, "image/jpeg", token);
                    await Task.WhenAll(t2, t3).ConfigureAwait(false);
                    return null;
                }
            }
            await t2.ConfigureAwait(false);
            return null;

        }

        protected async Task<IEnumerable<string>> UploadPreviewCacheToAzureAsync(Uri blobName,
            int startPage,
            Func<int, string> pageCacheBlobName,
            Func<int, Task<Stream>> convertPageToPreview, string cacheVersion,
            string mimeType, CancellationToken token
            )
        {
            if (pageCacheBlobName == null) throw new ArgumentNullException(nameof(pageCacheBlobName));
            if (convertPageToPreview == null) throw new ArgumentNullException(nameof(convertPageToPreview));
            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task>();

            var meta = await BlobProvider.FetchBlobMetaDataAsync(blobName, token).ConfigureAwait(false);
            meta = RemoveOldMetaTags(meta, cacheVersion);
            var pageCount = int.MaxValue;
            if (meta.TryGetValue(PagesInDocsMetaKey, out string sPageCount))
            {
                int.TryParse(sPageCount, out pageCount);
            }
            for (var pageIndex = startPage; pageIndex < Math.Min(startPage + NumberOfFilesInGroup, pageCount); pageIndex++)
            {
                var metaDataKey = cacheVersion + pageIndex;
                var cacheBlobName = pageCacheBlobName(pageIndex);
                if (GetPreviewImage(pageIndex, metaDataKey, ref meta, ref blobsNamesInCache, cacheBlobName))
                {
                    continue;
                }
                try
                {
                    using (var ms = await convertPageToPreview(pageIndex).ConfigureAwait(false))
                    {
                        var sr = await Compress.CompressToGzipAsync(ms).ConfigureAwait(false);
                        parallelTask.Add(BlobProviderCache.UploadByteArrayAsync(cacheBlobName, sr, mimeType, true, 30));
                        blobsNamesInCache.Add(BlobProviderCache.GenerateSharedAccessReadPermission(cacheBlobName, 30));
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
            await Task.WhenAll(tasks).ConfigureAwait(false);
            return blobsNamesInCache;
        }

        private bool GetPreviewImage(int pageIndex, string metaDataKey,
            ref IDictionary<string, string> meta,
            ref List<string> blobsNamesInCache,
            string cacheBlobName)
        {
            if (pageIndex > NumberOfFilesInGroup)
            {
                if (!BlobProviderCache.Exists(cacheBlobName)) return false;
                blobsNamesInCache.Add(BlobProviderCache.GenerateSharedAccessReadPermission(cacheBlobName, 20));
                return true;
            }
            if (!meta.TryGetValue(metaDataKey, out string _)) return false;
            blobsNamesInCache.Add(BlobProviderCache.GenerateSharedAccessReadPermission(cacheBlobName, 20));
            meta[metaDataKey] = DateTime.UtcNow.ToString(DatePattern);
            return true;
        }
    }
}
