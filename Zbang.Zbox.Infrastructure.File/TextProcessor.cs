using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public class TextProcessor : FileProcessor
    {
        private const string CacheVersion = CacheVersionPrefix + "1";

        private readonly IBlobProvider2<ICacheContainer> m_BlobProviderCache;
        public TextProcessor(IBlobProvider blobProvider, IBlobProvider2<ICacheContainer> blobProviderCache)
            : base(blobProvider)
        {
            m_BlobProviderCache = blobProviderCache;
        }

        public override async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            await UploadMetaDataAsync(blobUri, 1, CacheVersion, cancelToken).ConfigureAwait(false);
            return null;
        }



        public override async Task<PreviewResult> ConvertFileToWebsitePreviewAsync(Uri blobUri,
            int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = GetBlobNameFromUri(blobUri);
            var blobsNamesInCache = new List<string>();

            if (indexNum > 0)
            {
                return new PreviewResult { Content = blobsNamesInCache };
            }
            var cacheFileName = CreateCacheFileName(blobName);


            //var cacheBlobNameWithSharedAccessSignature = BlobProvider.GenerateSharedAccressReadPermissionInCache(cacheFileName, 20);

            if (await m_BlobProviderCache.ExistsAsync(cacheFileName).ConfigureAwait(false))
            {
                blobsNamesInCache.Add(m_BlobProviderCache.GenerateSharedAccessReadPermission(cacheFileName, 30));
                return new PreviewResult { ViewName = "Text", Content = blobsNamesInCache };
            }
            //if (IsFileExistsInCache(cacheBlobNameWithSharedAccessSignature))
            //{
            //    blobsNamesInCache.Add(cacheBlobNameWithSharedAccessSignature);
            //    return new PreviewResult { ViewName = "Text", Content = blobsNamesInCache };
            //}

            using (var stream = new StreamReader(await BlobProvider.DownloadFileAsync(blobUri, cancelToken).ConfigureAwait(false)))
            {
                var content = await stream.ReadToEndAsync().ConfigureAwait(false);
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    blobsNamesInCache.Add(await UploadFileToCacheAsync(ms, cacheFileName).ConfigureAwait(false));
                }
            }
            return new PreviewResult { Content = blobsNamesInCache, ViewName = "Text" };
        }



        public async Task<string> UploadFileToCacheAsync(Stream stream, string cacheFileName)
        {
            if (stream.Length == 0)
            {
                throw new ArgumentException("Stream is 0");
            }
            var sr = await Compress.CompressToGzipAsync(stream).ConfigureAwait(false);
            await m_BlobProviderCache.UploadByteArrayAsync(cacheFileName, sr, "text/plain", true, 30).ConfigureAwait(false);
            return m_BlobProviderCache.GenerateSharedAccessReadPermission(cacheFileName, 30);
            //var cacheName = await BlobProvider.UploadFileToCacheAsync(cacheFileName, sr, "text/plain", true);
            //return cacheName;
        }

        //private bool IsFileExistsInCache(string cacheBlobNameWithSharedAccessSignature)
        //{
        //    return !string.IsNullOrEmpty(cacheBlobNameWithSharedAccessSignature);
        //}

        private static string CreateCacheFileName(string blobName)
        {
            return $"{Path.GetFileNameWithoutExtension(blobName)}V3_{Path.GetExtension(blobName)}.html";
        }


        public static readonly string[] TxtExtensions = { ".txt", ".cpp", ".c", ".h", ".manifest", ".vcproj", ".java", ".sql", ".cs", ".css", ".less", ".log", ".vpp", ".xaml", ".xml", ".ini", ".suo", ".sln", ".php", ".js", ".config", ".htm", ".svg", ".html" };
        public override bool CanProcessFile(Uri blobName)
        {
            return blobName.AbsoluteUri.StartsWith(BlobProvider.StorageContainerUrl) && TxtExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        }

        public override async Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            using (var stream = new StreamReader(await BlobProvider.DownloadFileAsync(blobUri, cancelToken).ConfigureAwait(false)))
            {
                return await stream.ReadToEndAsync().ConfigureAwait(false);
            }
        }
    }
}
