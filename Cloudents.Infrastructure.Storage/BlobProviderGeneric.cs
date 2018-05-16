﻿using System;
using System.IO;
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
            // blob.Metadata.Add(LastAccessTimeMetaDataKey, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
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
        public async Task<string> MoveAsync(string blobName, string destinationContainerName)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentException("message", nameof(blobName));
            }

            //CloudBlockBlob destBlob;
            var destinationDirectory = _blobDirectory.GetDirectoryReference(destinationContainerName);
            var sourceBlob = GetBlob(blobName);

            //if (!destinationDirectory())
            //{
            //    throw new Exception("Destination container does not exist.");
            //}

            //Copy source blob to destination container
            //string name = srcBlob.Uri.Segments.Last();
            var destBlob = destinationDirectory.GetBlockBlobReference(blobName);
            await destBlob.StartCopyAsync(sourceBlob).ConfigureAwait(false);
            //remove source blob after copy is done.
            await sourceBlob.DeleteAsync().ConfigureAwait(false);
            return destBlob.Name;
        }

        //public async Task<CloudBlockBlob> UploadBlobAsync(Stream data,
        //    string filename, bool compressed = true, int? cacheControlMinutes = null, CancellationToken token = default)
        //{

        //    //string origMD5 = MD5(data);
        //    var blob = GetBlob(filename);

        //    if (compressed)
        //    {
        //        using (var stream = await blob.OpenWriteAsync().ConfigureAwait(false))
        //        using (GZipStream gzip = new GZipStream(stream, CompressionLevel.Optimal))
        //        {
        //            await data.CopyToAsync(gzip, 81920);
        //            //await gzip.WriteAsync(data, 0, data.Length, token).ConfigureAwait(false);
        //        }
        //        //data = comp.ToArray();
        //        blob.Properties.ContentEncoding = "gzip";
        //        await blob.SetPropertiesAsync();
        //    }

        //    //blob.Metadata.Add("compressed", compressed.ToString());
        //    //blob.Metadata.Add("origMD5", origMD5);
        //    await blob.UploadFromByteArrayAsync(data, 0, data.Length).ConfigureAwait(false);
        //    return blob;
        //}


    }
}
