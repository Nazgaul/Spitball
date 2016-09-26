using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Infrastructure.Azure.Blob
{
    public class BlobProvider2<T> : BlobProvider, IBlobProvider2<T> where T : IStorageContainerName, new()
    {
        //private CloudBlobClient m_BlobClient;
        // private readonly ILocalStorageProvider m_LocalStorageProvider;
        private readonly T m_Container = new T();

        public BlobProvider2(ILocalStorageProvider localStorageProvider)
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
            }, null);
            return ++currentIndex;
        }

        public async Task CommitBlockListAsync(string blobName, int currentIndex, string contentType)
        {
            var blockList = Enumerable.Range(0, currentIndex).Select(ToBase64);
            var blob = GetBlob(blobName);
            blob.Properties.ContentType = contentType;
            blob.Properties.CacheControl = "private max-age=" + TimeConst.Week;
            await blob.PutBlockListAsync(blockList, null, new BlobRequestOptions
            {
                StoreBlobContentMD5 = true

            }, null);

        }

        public Task UploadStreamAsync(string blobName, Stream content, string mimeType, CancellationToken token)
        {
            if (content.CanSeek)
            {
                content.Seek(0, SeekOrigin.Begin);
            }
            var blob = GetBlob(blobName);
            //var blob = BlobClient.GetContainerReference(AzurePreviewContainer).GetBlockBlobReference(blobName);
            blob.Properties.ContentType = mimeType;
            blob.Properties.CacheControl = "public, max-age=" + TimeConst.Year;
            return blob.UploadFromStreamAsync(content, token);
        }

        public Uri GetBlobUrl(string blobName)
        {
            return GetBlob(blobName).Uri;
        }

        public Task<bool> ExistsAsync(string blobName)
        {
            var blob = GetBlob(blobName);
            return blob.ExistsAsync();
        }
        public bool Exists(string blobName)
        {
            var blob = GetBlob(blobName);
            return blob.Exists();
        }

        public async Task<long> SizeAsync(string blobName)
        {
            var blob = GetBlob(blobName);
            await blob.FetchAttributesAsync();
            return blob.Properties.Length;
        }

        public async Task<string> Md5Async(string blobName)
        {
            var blob = GetBlob(blobName);
            await blob.FetchAttributesAsync();
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

                using (var sr = await client.GetAsync(url))
                {
                    if (!sr.IsSuccessStatusCode)
                    {
                        throw new UnauthorizedAccessException("Cannot access dropbox");
                    }
                    ////sr.Content.Headers.ContentType.
                    var blob = GetFile(fileName);
                    using (var stream = await blob.OpenWriteAsync())
                    {
                        await sr.Content.CopyToAsync(stream);

                    }
                    blob.Properties.ContentType = sr.Content.Headers.ContentType.MediaType;
                    blob.Properties.CacheControl = "private max-age=" + TimeConst.Week;
                    await blob.SetPropertiesAsync();
                }
            }
        }


        public async Task UploadByteArrayAsync(string blobName, byte[] fileContent,
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
            await blob.UploadFromByteArrayAsync(fileContent, 0, fileContent.Length);
        }

        public string GenerateSharedAccressReadPermission(string blobName, double expirationTimeInMinutes)
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

        public string GenerateSharedAccressReadPermission(string blobName, double expirationTimeInMinutes, string contentDisposition)
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

        public string GenerateSharedAccressWritePermission(string blobName, string mimeType)
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