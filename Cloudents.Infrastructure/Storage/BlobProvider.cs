using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.Infrastructure.Storage
{
    public class BlobProvider : IBlobProvider
    {
        private readonly CloudStorageProvider _storageProvider;

        public BlobProvider(CloudStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        private CloudBlockBlob GetBlob(Uri blobUrl)
        {
            return new CloudBlockBlob(blobUrl, _storageProvider.CloudStorage.Credentials);
        }

        public async Task<Stream> DownloadFileAsync(Uri blobUrl, CancellationToken token)
        {
            var blob = GetBlob(blobUrl);
            var ms = new MemoryStream();
            await blob.DownloadToStreamAsync(ms).ConfigureAwait(false);
            //await blob.DownloadToStreamAsync(ms, cancelToken).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public async Task<IDictionary<string, string>> FetchBlobMetaDataAsync(Uri blobUri, CancellationToken token)
        {
            var blob = GetBlob(blobUri);// GetFile(blobName);
            await blob.FetchAttributesAsync().ConfigureAwait(false);
            return blob.Metadata;
        }

        public  string GetBlobNameFromUri(Uri uri)
        {
            return uri.Segments[uri.Segments.Length - 1];
        }

        public Task SaveMetaDataToBlobAsync(Uri blobUri, IDictionary<string, string> metadata, CancellationToken token)
        {
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            var blob = GetBlob(blobUri);
            foreach (var item in metadata)
            {
                blob.Metadata[item.Key] = item.Value;
            }
            return blob.SetMetadataAsync();
        }
    }
}
