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
        const string CacheVersion = CacheVersionPrefix + "1";
        public TextProcessor(IBlobProvider blobProvider, IBlobProvider2<IStorageContainerName> blobProviderPreview)
            : base(blobProvider, blobProviderPreview)
        {

        }
        public override async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            await UploadMetaDataAsync(blobUri, 1, CacheVersion, cancelToken);
            return null;
            //using (var stream = new StreamReader(await BlobProvider.DownloadFileAsync(blobName, cancelToken)))
            //{
            //    var sb = new StringBuilder();
            //    while (stream.Peek() > 0)
            //    {
            //        sb.AppendLine(await stream.ReadLineAsync());
            //        if (sb.Length > 10000)
            //        {
            //            break;
            //        }
            //    }
            //    await UploadMetaDataAsync(blobName, 1, CacheVersion, cancelToken);
            //    return PreProcessFileResult.GetEmptyResult;
            //    //return new PreProcessFileResult
            //    //{
            //    //    FileTextContent = content
            //    //};
            //}

        }



        public override async Task<PreviewResult> ConvertFileToWebSitePreviewAsync(Uri blobUri,
            int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = GetBlobNameFromUri(blobUri);
            var blobsNamesInCache = new List<string>();

            if (indexNum > 0)
            {
                return new PreviewResult { Content = blobsNamesInCache };
            }
            var cacheFileName = CreateCacheFileName(blobName);

            var cacheBlobNameWithSharedAccessSignature = BlobProvider.GenerateSharedAccressReadPermissionInCache(cacheFileName, 20);

            if (IsFileExistsInCache(cacheBlobNameWithSharedAccessSignature))
            {
                blobsNamesInCache.Add(cacheBlobNameWithSharedAccessSignature);
                return new PreviewResult { ViewName = "Text", Content = blobsNamesInCache };
            }

            using (var stream = new StreamReader(await BlobProvider.DownloadFileAsync(blobUri, cancelToken)))
            {
                var content = await stream.ReadToEndAsync();
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    blobsNamesInCache.Add(await UploadFileToCacheAsync(ms, cacheFileName));
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
            var compressor = new Compress();
            var sr = compressor.CompressToGzip(stream);
            var cacheName = await BlobProvider.UploadFileToCacheAsync(cacheFileName, sr, "text/plain", true);
            return cacheName;
        }

        private bool IsFileExistsInCache(string cacheBlobNameWithSharedAccessSignature)
        {
            return !string.IsNullOrEmpty(cacheBlobNameWithSharedAccessSignature);
        }

        private string CreateCacheFileName(string blobName)
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
            var blobName = GetBlobNameFromUri(blobUri);
            using (var stream = new StreamReader(await BlobProvider.DownloadFileAsync(blobUri, cancelToken)))
            {
                return stream.ReadToEnd();
            }
        }

        //public override Task GenerateImagePreviewAsync(Uri blobUri, CancellationToken cancelToken)
        //{
        //    return Extensions.TaskExtensions.CompletedTask;

        //}
    }
}
