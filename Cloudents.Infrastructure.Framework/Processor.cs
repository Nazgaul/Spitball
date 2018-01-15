using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Framework
{
    public abstract class Processor
    {
        protected readonly IBlobProvider BlobProvider;
        protected readonly IBlobProvider<CacheContainer> BlobProviderCache;
        protected readonly Uri BlobUri;

        public const string CacheVersionPrefix = "V";
        protected const string PagesInDocsMetaKey = "pageCount";
        protected const int NumberOfFilesInGroup = 15;
        protected const string DatePattern = "M-d-yy";

        protected Processor(IBlobProvider blobProvider, IBlobProvider<CacheContainer> blobProviderCache, Uri blobUri)
        {
            BlobProvider = blobProvider;
            BlobProviderCache = blobProviderCache;
            BlobUri = blobUri;
        }

       // public abstract bool CanProcessFile(Uri blobName);


        protected async Task<IEnumerable<string>> UploadPreviewCacheToAzureAsync(
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

            var meta = await BlobProvider.FetchBlobMetaDataAsync(BlobUri, token).ConfigureAwait(false);
            //meta = RemoveOldMetaTags(meta, cacheVersion);
            var pageCount = int.MaxValue;
            if (meta.TryGetValue(PagesInDocsMetaKey, out string sPageCount))
            {
                int.TryParse(sPageCount, out pageCount);
            }
            for (var pageIndex = startPage; pageIndex < Math.Min(startPage + NumberOfFilesInGroup, pageCount); pageIndex++)
            {
                var metaDataKey = cacheVersion + pageIndex;
                var cacheBlobName = pageCacheBlobName(pageIndex);
                var previewImageLink = await GetPreviewImageAsync(pageIndex, metaDataKey, meta, cacheBlobName, token);
                if (!string.IsNullOrEmpty(previewImageLink))
                {
                    blobsNamesInCache.Add(previewImageLink);
                    continue;
                }
                try
                {
                    using (var ms = await convertPageToPreview(pageIndex).ConfigureAwait(false))
                    {
                        var sr = await Compress.CompressToGzipAsync(ms, token).ConfigureAwait(false);
                        parallelTask.Add(BlobProviderCache.UploadByteArrayAsync(cacheBlobName, sr, mimeType, true, 30, token));
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
            var t = BlobProvider.SaveMetaDataToBlobAsync(BlobUri, meta, token);
            var tasks = new List<Task>();
            tasks.AddRange(parallelTask);
            tasks.Add(t);
            await Task.WhenAll(tasks).ConfigureAwait(false);
            return blobsNamesInCache;
        }

        private async Task<string> GetPreviewImageAsync(int pageIndex, string metaDataKey,
            IDictionary<string, string> meta,
            //ref List<string> blobsNamesInCache,
            string cacheBlobName, CancellationToken token)
        {
            if (pageIndex > NumberOfFilesInGroup)
            {
                if (!await BlobProviderCache.ExistsAsync(cacheBlobName, token).ConfigureAwait(false))
                {
                    return null;
                }
                return BlobProviderCache.GenerateSharedAccessReadPermission(cacheBlobName, 20);
            }
            if (!meta.TryGetValue(metaDataKey, out string _)) return null;
            meta[metaDataKey] = DateTime.UtcNow.ToString(DatePattern);
            return BlobProviderCache.GenerateSharedAccessReadPermission(cacheBlobName, 20);
        }


        //protected static IDictionary<string, string> RemoveOldMetaTags(IDictionary<string, string> metaTags, string cacheVersionPrefix)
        //{
        //    var oldElements = metaTags.Where(w =>
        //        Regex.IsMatch(w.Key, @"\d") && !w.Key.StartsWith(cacheVersionPrefix)).Select(s => s.Key).ToList();

        //    foreach (var oldElement in oldElements)
        //    {
        //        metaTags.Remove(oldElement);
        //    }

        //    var z = metaTags.Select(s => s.Key).Where(w => w.StartsWith(cacheVersionPrefix)).Select(s =>
        //    {
        //        var number = s.Remove(0, cacheVersionPrefix.Length);
        //        return Convert.ToInt32(number);
        //    }).Where(p => p >= 15).ToList();

        //    foreach (int i in z)
        //    {
        //        metaTags.Remove(cacheVersionPrefix + i);
        //    }

        //    return metaTags;
        //}

    }
}
