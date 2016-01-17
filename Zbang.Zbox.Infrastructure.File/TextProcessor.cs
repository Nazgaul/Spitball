﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public class TextProcessor : FileProcessor
    {
        const string CacheVersion = CacheVersionPrefix + "1";
        public TextProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {

        }
        public override async Task<PreProcessFileResult> PreProcessFile(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = GetBlobNameFromUri(blobUri);
            using (var stream = new StreamReader(await BlobProvider.DownloadFileAsync(blobName, cancelToken)))
            {
                var sb = new StringBuilder();
                while (stream.Peek() > 0)
                {
                    sb.AppendLine(await stream.ReadLineAsync());
                    if (sb.Length > 10000)
                    {
                        break;
                    }
                }
                var content = sb.ToString();
                content = StripUnwantedChars(content);
                await UploadMetaData(blobName, 1, CacheVersion);
                return new PreProcessFileResult
                {
                    ThumbnailName = GetDefaultThumbnailPicture(),
                    FileTextContent = content
                };
            }

        }

        public override string GetDefaultThumbnailPicture()
        {
            // We don't use svg because we need the entire system to support this
            return DefaultPicture.TextFileTypePicture;
        }

        public override async Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, 
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

            using (var stream = new StreamReader(await BlobProvider.DownloadFileAsync(blobName, cancelToken)))
            {
                var content = await stream.ReadToEndAsync();
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    blobsNamesInCache.Add(await UploadFileToCache(ms, cacheFileName));
                }
            }
            return new PreviewResult { Content = blobsNamesInCache, ViewName = "Text" };
        }



        public async Task<string> UploadFileToCache(Stream stream, string cacheFileName)
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
            return string.Format("{0}V3_{1}.html", Path.GetFileNameWithoutExtension(blobName), Path.GetExtension(blobName));
        }


        public static readonly string[] TxtExtensions = { ".txt", ".cpp", ".c", ".h", ".manifest", ".vcproj", ".java", ".sql", ".cs", ".css", ".less", ".log", ".vpp", ".xaml", ".xml", ".ini", ".suo", ".sln", ".php", ".js", ".config", ".htm", ".svg", ".html" };
        public override bool CanProcessFile(Uri blobName)
        {
            return blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl) && TxtExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        }

        public override async Task<string> ExtractContent(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = GetBlobNameFromUri(blobUri);
            using (var stream = new StreamReader(await BlobProvider.DownloadFileAsync(blobName, cancelToken)))
            {
                return stream.ReadToEnd();
            }
        }
    }
}
