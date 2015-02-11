using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public abstract class DocumentProcessor : FileProcessor
    {
        protected const int NumberOfFilesInGroup = 15;
        protected const string DatePattern = "M-d-yy";



        protected DocumentProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        { }

        protected async Task<PreProcessFileResult> ProcessFile(string blobName,
            Func<Stream> thumbnailStream,
            Func<String> extractTextFromDocument,
            Func<int> getPageCount,
            String getCacheVersionPrefix
            )
        {
            var thumbnailUri = Path.GetFileNameWithoutExtension(blobName) + ".thumbnailV3.jpg";
            var text = extractTextFromDocument();
            using (var ms = thumbnailStream())
            {
                var t1 = BlobProvider.UploadFileThumbnailAsync(thumbnailUri, ms, "image/jpeg");
                var t2 = UploadMetaData(text, blobName, getPageCount(), getCacheVersionPrefix);
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
            Func<int, string> pageCacheBlobName,
            Func<int, Task<Stream>> convertPageToPreview, string cacheVersion,
            String mimeType
            )
        {
            var blobsNamesInCache = new List<string>();
            var parallelTask = new List<Task<string>>();

            var meta = await BlobProvider.FetechBlobMetaDataAsync(blobName);
            meta = RemoveOldMetaTags(meta, cacheVersion);
            string sPageCount;
            int pageCount = int.MaxValue;
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
                        parallelTask.Add(BlobProvider.UploadFileToCacheAsync(cacheblobName, sr, mimeType, true));
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
            var t = BlobProvider.SaveMetaDataToBlobAsync(blobName, meta);
            var tasks = new List<Task>();
            tasks.AddRange(parallelTask);
            tasks.Add(t);
            await Task.WhenAll(tasks);
            blobsNamesInCache.AddRange(parallelTask.Select(s => s.Result));
            return blobsNamesInCache;
        }

        private bool GetPrviewImage(int pageIndex, string metaDataKey,
            ref IDictionary<string, string> meta,
            ref List<string> blobsNamesInCache,
            string cacheblobName)
        {
            if (pageIndex > NumberOfFilesInGroup)
            {
                if (BlobProvider.CacheBlobExists(cacheblobName))
                {
                    blobsNamesInCache.Add(BlobProvider.GenerateSharedAccressReadPermissionInCacheWithoutMeta(cacheblobName, 20));
                    return true;
                }
            }
            else
            {
                string value;
                if (meta.TryGetValue(metaDataKey, out value))
                {
                    blobsNamesInCache.Add(BlobProvider.GenerateSharedAccressReadPermissionInCacheWithoutMeta(cacheblobName, 20));
                    meta[metaDataKey] = DateTime.UtcNow.ToString(DatePattern);
                    return true;
                }
            }
            return false;
        }
    }
}
