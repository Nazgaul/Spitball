using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Framework
{
    public class TextProcessor : IPreviewProvider
    {
        //private const string CacheVersion = Processor.CacheVersionPrefix + "1";
        private readonly IBlobProvider _blobProvider;
        private readonly Uri _blobUri;
        private readonly IBlobProvider<CacheContainer> _blobProviderCache;


        public TextProcessor(IBlobProvider blobProvider, 
            Uri blobUri,
            IBlobProvider<CacheContainer> blobProviderCache)
        {
            _blobProvider = blobProvider;
            _blobUri = blobUri;
            _blobProviderCache = blobProviderCache;
        }
        //private readonly IBlobProvider2<ICacheContainer> m_BlobProviderCache;
        //public TextProcessor(IBlobProvider blobProvider, IBlobProvider2<ICacheContainer> blobProviderCache)
        //    : base(blobProvider)
        //{
        //    m_BlobProviderCache = blobProviderCache;
        //}

        //public override async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        //{
        //    await UploadMetaDataAsync(blobUri, 1, CacheVersion, cancelToken).ConfigureAwait(false);
        //    return null;
        //}

        public async Task<IEnumerable<string>> ConvertFileToWebsitePreviewAsync(
            int indexNum, CancellationToken cancelToken)
        {
            var blobName = _blobProvider.GetBlobNameFromUri(_blobUri);

            if (indexNum > 0)
            {
                return Enumerable.Empty<string>();
            }
            var blobsNamesInCache = new List<string>();

            var cacheFileName = CreateCacheFileName(blobName);

            if (await _blobProviderCache.ExistsAsync(cacheFileName, cancelToken).ConfigureAwait(false))
            {
                blobsNamesInCache.Add(_blobProviderCache.GenerateSharedAccessReadPermission(cacheFileName, 30));
                return blobsNamesInCache;
            }

            using (var stream = new StreamReader(await _blobProvider.DownloadFileAsync(_blobUri, cancelToken).ConfigureAwait(false)))
            {
                var content = await stream.ReadToEndAsync().ConfigureAwait(false);
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    blobsNamesInCache.Add(await UploadFileToCacheAsync(ms, cacheFileName, cancelToken).ConfigureAwait(false));
                }
            }
            return blobsNamesInCache;
        }

        public async Task<string> UploadFileToCacheAsync(Stream stream, string cacheFileName, CancellationToken token)
        {
            if (stream.Length == 0)
            {
                throw new ArgumentException("Stream is 0");
            }
            var sr = await Compress.CompressToGzipAsync(stream, token).ConfigureAwait(false);
            await _blobProviderCache.UploadStreamAsync(cacheFileName, sr, "text/plain", true, 30, token).ConfigureAwait(false);
            return _blobProviderCache.GenerateSharedAccessReadPermission(cacheFileName, 30);
        }

        private static string CreateCacheFileName(string blobName)
        {
            return $"{Path.GetFileNameWithoutExtension(blobName)}V3_{Path.GetExtension(blobName)}.html";
        }

        public static readonly string[] TxtExtensions = { ".txt", ".cpp", ".c", ".h", ".manifest", ".vcproj", ".java", ".sql", ".cs", ".css", ".less", ".log", ".vpp", ".xaml", ".xml", ".ini", ".suo", ".sln", ".php", ".js", ".config", ".htm", ".svg", ".html" };


        //public override bool CanProcessFile(Uri blobName)
        //{
        //    return blobName.AbsoluteUri.StartsWith(BlobProvider.StorageContainerUrl) && TxtExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        //}

        //public override async Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        //{
        //    using (var stream = new StreamReader(await BlobProvider.DownloadFileAsync(blobUri, cancelToken).ConfigureAwait(false)))
        //    {
        //        return await stream.ReadToEndAsync().ConfigureAwait(false);
        //    }
        //}
    }
}
