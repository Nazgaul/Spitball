using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Extension;
using Cloudents.Core.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.Infrastructure.Storage
{
    public class BlobProviderContainer<T> : IBlobProvider<T> where T : IStorageContainer, new()
    {
        private readonly CloudBlobDirectory _blobDirectory;
        private readonly T _container = new T();

        private const string CdnHostEndpoint = "az32006.vo.msecnd.net";


        public BlobProviderContainer( ICloudStorageProvider storageProvider)
        {
            _blobDirectory = storageProvider.GetBlobClient(_container);
        }

        //public BlobProvider(StorageContainer container, ICloudStorageProvider storageProvider)
        //{
        //    _blobDirectory = storageProvider.GetBlobClient(container);
        //    _storageProvider = storageProvider;
        //}

        public Uri GetBlobUrl(string blobName, bool cdn = false)
        {
            var blob = _blobDirectory.GetBlockBlobReference(blobName);
            var uri = blob.Uri;
            if (cdn)
            {
                uri = uri.ChangeHost(CdnHostEndpoint);
            }
            return uri;
        }

        private CloudBlockBlob GetBlob(string blobName)
        {
            return _blobDirectory.GetBlockBlobReference(blobName);
        }


        public Task UploadStreamAsync(string blobName, Stream fileContent,
            string mimeType, bool fileGziped, int cacheControlMinutes, CancellationToken token)
        {
            var blob = GetBlob(blobName);
            fileContent.Seek(0, SeekOrigin.Begin);
            blob.Properties.ContentType = mimeType;
            if (fileGziped)
            {
                blob.Properties.ContentEncoding = "gzip";
            }
            blob.Properties.CacheControl = "private, max-age=" + (TimeConst.Minute * cacheControlMinutes);
            return blob.UploadFromStreamAsync(fileContent);
        }

        public string GenerateSharedAccessReadPermission(string blobName, double expirationTimeInMinutes)
        {
            return GenerateSharedAccessReadPermission(blobName, expirationTimeInMinutes, null);
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
                ContentDisposition = contentDisposition ?? string.Empty
            });
            var url = new Uri(blob.Uri, signedUrl);
            return url.AbsoluteUri;
        }

        public Task<bool> ExistsAsync(string blobName, CancellationToken token)
        {
            var blob = GetBlob(blobName);
            return blob.ExistsAsync();
        }

        public async Task MoveAsync(string blobName, string destinationContainerName, CancellationToken token)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentException("message", nameof(blobName));
            }
            var destinationDirectory = _blobDirectory.GetDirectoryReference(destinationContainerName);
            var sourceBlob = GetBlob(blobName);
            var destinationBlob = destinationDirectory.GetBlockBlobReference(blobName);
            await destinationBlob.StartCopyAsync(sourceBlob).ConfigureAwait(false);
            await sourceBlob.DeleteAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Uri>> FilesInDirectoryAsync(string directory, CancellationToken token)
        {
            var destinationDirectory = _blobDirectory.GetDirectoryReference(directory);

            var result = await destinationDirectory.ListBlobsSegmentedAsync(true, BlobListingDetails.None, 1000, null, null, null, token).ConfigureAwait(false);
            return result.Results.Select(s => s.Uri);

        }
    }
}
