using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.Infrastructure.Storage
{
    public class BlobProvider<T> : IBlobProvider<T> where T : IStorageContainer, new()
    {
        private readonly CloudBlobDirectory _blobDirectory;
        private readonly T _container = new T();

        public BlobProvider(CloudStorageProvider storageProvider)
        {
            var blobClient = storageProvider.CloudStorage.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(_container.Name.ToLowerInvariant());
            _blobDirectory = container.GetDirectoryReference(_container.RelativePath);
        }

        public Uri GetBlobUrl(string blobName)
        {
            var blob = _blobDirectory.GetBlockBlobReference(blobName);
            return blob.Uri;
        }

        private CloudBlockBlob GetBlob(string blobName)
        {
            return _blobDirectory.GetBlockBlobReference(blobName);
        }

        public Task UploadByteArrayAsync(string blobName, byte[] fileContent,
            string mimeType, bool fileGziped, int cacheControlMinutes, CancellationToken token)
        {
            var blob = GetBlob(blobName);
            //fileContent.Seek(0, SeekOrigin.Begin);

            blob.Properties.ContentType = mimeType;
            if (fileGziped)
            {
                blob.Properties.ContentEncoding = "gzip";
            }

            blob.Properties.CacheControl = "private, max-age=" + (TimeConst.Minute * cacheControlMinutes);
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


        public Task<bool> ExistsAsync(string blobName, CancellationToken token)
        {
            var blob = GetBlob(blobName);
            return blob.ExistsAsync();
        }

    }
}
