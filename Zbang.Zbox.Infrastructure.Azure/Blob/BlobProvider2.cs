using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.Azure.Blob
{
    public class BlobProvider2<T> : BlobProvider, IBlobProvider2<T> where T : IStorageContainerName, new()
    {
        private readonly T m_Container = new T();

        //There is some race condition with initialize until the localStorage provider is actually initialized
        public BlobProvider2(Lazy<ILocalStorageProvider> localStorageProvider)
            : base(localStorageProvider)
        {
        }

        private CloudBlockBlob GetBlob(string blobName)
        {
            var container = BlobClient.GetContainerReference(m_Container.Name.ToLower());
            var directory = container.GetDirectoryReference(m_Container.RelativePath);
            return directory.GetBlockBlobReference(blobName);
        }

        public async Task<int> UploadFileBlockAsync(string blobName, Stream fileContent, int currentIndex)
        {
            var blob = GetBlob(blobName);
            fileContent.Seek(0, SeekOrigin.Begin);
            await blob.PutBlockAsync(ToBase64(currentIndex), fileContent, null, null, new BlobRequestOptions
            {
                StoreBlobContentMD5 = true
            }, null).ConfigureAwait(false);
            return ++currentIndex;
        }

        public Task CommitBlockListAsync(string blobName, int currentIndex, string contentType)
        {
            var blockList = Enumerable.Range(0, currentIndex).Select(ToBase64);
            var blob = GetBlob(blobName);
            blob.Properties.ContentType = contentType;
            blob.Properties.CacheControl = "private max-age=" + TimeConst.Week;
            return blob.PutBlockListAsync(blockList, null, new BlobRequestOptions
            {
                StoreBlobContentMD5 = true

            }, null);
        }

        public Task UploadStreamAsync(string blobName, Stream content, string mimeType, CancellationToken token)
        {
            if (blobName == null) throw new ArgumentNullException(nameof(blobName));
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (content.CanSeek)
            {
                content.Seek(0, SeekOrigin.Begin);
            }
            var blob = GetBlob(blobName);
            blob.Properties.ContentType = mimeType;
            blob.Properties.CacheControl = "public, max-age=" + TimeConst.Year;
            return blob.UploadFromStreamAsync(content, token);
        }

        public void UploadText(string blobName, string content)
        {
            if (blobName == null) throw new ArgumentNullException(nameof(blobName));
            if (content == null) throw new ArgumentNullException(nameof(content));
            var blob = GetBlob(blobName);
            blob.UploadText(content);
        }

        public Task<string> DownloadTextAsync(string blobName)
        {
            if (blobName == null) throw new ArgumentNullException(nameof(blobName));
            var blob = GetBlob(blobName);
            return blob.DownloadTextAsync();
        }

        public Task RemoveBlobAsync(string blobName, CancellationToken token)
        {
            var blob = GetBlob(blobName);
            return blob.DeleteIfExistsAsync(token);
        }

        public Uri GetBlobUrl(string blobName)
        {
            return GetBlob(blobName).Uri;
        }

        public Uri GetBlobUrl(string blobName, bool useCdn)
        {
            var uri = GetBlobUrl(blobName);
            if (string.IsNullOrEmpty(StorageCdnEndpoint))
            {
                return uri;
            }
            var path = uri.PathAndQuery;
            if (path.StartsWith("/"))
            {
                path = path.Remove(0, 1);
            }
            var uriBuilder = new UriBuilder(StorageCdnEndpoint) {Path = path};

            return uriBuilder.Uri;
        }

        public Task<bool> ExistsAsync(string blobName, CancellationToken token)
        {
            var blob = GetBlob(blobName);
            return blob.ExistsAsync(token);
        }

        public bool Exists(string blobName)
        {
            var blob = GetBlob(blobName);
            return blob.Exists();
        }

        public async Task<long> SizeAsync(string blobName)
        {
            var blob = GetBlob(blobName);
            await blob.FetchAttributesAsync().ConfigureAwait(false);
            return blob.Properties.Length;
        }

        public async Task<string> MD5Async(string blobName)
        {
            var blob = GetBlob(blobName);
            await blob.FetchAttributesAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(blob.Properties.ContentMD5))
            {
                var buffer = new byte[16 * 1024];
                var md5Object = System.Security.Cryptography.MD5.Create();
                using (var stream = await blob.OpenReadAsync().ConfigureAwait(false))
                {
                    int bytesReceived;
                    while ((bytesReceived = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
                    {
                        md5Object.TransformBlock(buffer, 0, bytesReceived, null, 0);
                    }
                    md5Object.TransformFinalBlock(new byte[0], 0, 0);
                    blob.Properties.ContentMD5 = Convert.ToBase64String(md5Object.Hash);
                    await blob.SetPropertiesAsync().ConfigureAwait(false);
                }
            }
            return blob.Properties.ContentMD5;
        }

        public string RelativePath()
        {
            return m_Container.RelativePath;
        }

        /// <summary>
        /// Upload file to storage from link
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <returns>The size of the file</returns>
        public async Task UploadFromLinkAsync(string url, string fileName)
        {
            using (var client = new HttpClient())
            {
                using (var sr = await client.GetAsync(url).ConfigureAwait(false))
                {
                    if (!sr.IsSuccessStatusCode)
                    {
                        throw new UnauthorizedAccessException("Cannot access dropBox");
                    }
                    ////sr.Content.Headers.ContentType.
                    var blob = GetFile(fileName);
                    using (var stream = await blob.OpenWriteAsync().ConfigureAwait(false))
                    {
                        await sr.Content.CopyToAsync(stream).ConfigureAwait(false);
                    }
                    blob.Properties.ContentType = sr.Content.Headers.ContentType.MediaType;
                    blob.Properties.CacheControl = "private max-age=" + TimeConst.Week;
                    await blob.SetPropertiesAsync().ConfigureAwait(false);
                }
            }
        }

        public Task UploadByteArrayAsync(string blobName, byte[] fileContent,
            string mimeType, bool fileGziped, int cacheControlMinutes)
        {
            var blob = GetBlob(blobName);
            //fileContent.Seek(0, SeekOrigin.Begin);

            blob.Properties.ContentType = mimeType;
            if (fileGziped)
            {
                blob.Properties.ContentEncoding = "gzip";
            }

            blob.Properties.CacheControl = "private, max-age=" + TimeConst.Minute * cacheControlMinutes;
            // blob.Metadata.Add(LastAccessTimeMetaDataKey, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            return blob.UploadFromByteArrayAsync(fileContent, 0, fileContent.Length);
        }

        public string GenerateSharedAccessReadPermission(string blobName, double expirationTimeInMinutes)
        {
            var blob = GetBlob(blobName);

            var signedUrl = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(expirationTimeInMinutes)
            });
            var url = new Uri(blob.Uri, signedUrl);
            return url.AbsoluteUri;
        }

        public string GenerateSharedAccessReadPermission(string blobName, double expirationTimeInMinutes, string contentDisposition)
        {
            var blob = GetBlob(blobName);

            var signedUrl = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(expirationTimeInMinutes)

            }, new SharedAccessBlobHeaders
            {
                ContentDisposition = contentDisposition
            });
            var url = new Uri(blob.Uri, signedUrl);
            return url.AbsoluteUri;
        }

        public string GenerateSharedAccessWritePermission(string blobName, string mimeType)
        {
            var blob = GetBlob(blobName);
            var queryString = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Write,
                SharedAccessExpiryTime = DateTime.Now.AddHours(2)
            }, new SharedAccessBlobHeaders
            {
                ContentType = mimeType,
                CacheControl = "private max-age=604800"
            });
            return blob.Uri + queryString;
        }
    }
}