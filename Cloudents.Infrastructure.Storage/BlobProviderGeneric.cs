using Cloudents.Core.Extension;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Storage
{
    public class BlobProviderContainer : IBlobProvider,
        IChatDirectoryBlobProvider
    {
        protected readonly CloudBlobDirectory _blobDirectory;
        private readonly CloudBlobContainer _cloudContainer;
        private readonly StorageContainer _container;
        private readonly CloudBlobClient _client;
        private const string CdnHostEndpoint = "az32006.vo.msecnd.net";


        public BlobProviderContainer(ICloudStorageProvider storageProvider)
        {
            _client = storageProvider.GetBlobClient();
        }

        public BlobProviderContainer(ICloudStorageProvider storageProvider, StorageContainer container)
        {

            _client = storageProvider.GetBlobClient();
            _cloudContainer = _client.GetContainerReference(container.Name.ToLowerInvariant());
            _container = container;
            _blobDirectory = _cloudContainer.GetDirectoryReference(container.RelativePath ?? string.Empty);
        }



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

        private CloudBlockBlob GetBlob(Uri blobUrl)
        {

            return new CloudBlockBlob(blobUrl, _client.Credentials);
        }

        private CloudBlockBlob GetBlob(string blobName)
        {
            return _blobDirectory.GetBlockBlobReference(blobName);
        }


        public Task UploadStreamAsync(string blobName, Stream fileContent,
            string mimeType = null, TimeSpan? cacheControlSeconds = null, CancellationToken token = default)
        {
            var blob = GetBlob(blobName);
            if (fileContent.CanSeek)
            {
                fileContent.Seek(0, SeekOrigin.Begin);
                //throw new ArgumentException("stream should need to be able to seek");
            }
            if (mimeType != null)
            {
                blob.Properties.ContentType = mimeType;
            }

            //if (fileGziped)
            //{
            //    blob.Properties.ContentEncoding = "gzip";
            //}

            if (cacheControlSeconds.HasValue)
            {
                blob.Properties.CacheControl = "private, max-age=" + cacheControlSeconds.Value.TotalSeconds;
            }

            return blob.UploadFromStreamAsync(fileContent);
        }

        public Task UploadBlockFileAsync(string blobName, Stream fileContent, int index, CancellationToken token)
        {
            var blob = GetBlob(blobName);
            fileContent.Seek(0, SeekOrigin.Begin);
            return blob.PutBlockAsync(ToBase64(index), fileContent, null, null, new BlobRequestOptions
            {
                StoreBlobContentMD5 = true
            }, null, token);
        }

        //public Task CommitBlockListAsync(string blobName, IList<int> indexes, CancellationToken token)
        //{
        //    var blob = GetBlob(blobName);
        //    return blob.PutBlockListAsync(indexes.Select(ToBase64));
        //}

        public Task CommitBlockListAsync(string blobName, string mimeType, IList<int> indexes, CancellationToken token)
        {
            return CommitBlockListAsync(blobName, mimeType, null, indexes, token);
        }

        public Task CommitBlockListAsync(string blobName, string mimeType, string originalFileName, IList<int> indexes,
            CancellationToken token)
        {
            var blob = GetBlob(blobName);
            blob.Properties.ContentType = mimeType;
            if (!string.IsNullOrEmpty(originalFileName))
            {
                blob.Metadata["fileName"] = originalFileName;
            }

            return blob.PutBlockListAsync(indexes.Select(ToBase64));
        }

        private static string ToBase64(int blockIndex)
        {
            var blockId = blockIndex.ToString("D10");
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(blockId));
        }





        //public async Task<string> DownloadTextAsync(string name, string directory, CancellationToken token)
        //{
        //    try
        //    {
        //        var destinationDirectory = _blobDirectory.GetDirectoryReference(directory);
        //        var blob = destinationDirectory.GetBlockBlobReference(name);

        //        return await blob.DownloadTextAsync();
        //    }
        //    catch (StorageException e)
        //    {
        //        if (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
        //        {
        //            return null;
        //        }
        //        throw;
        //    }
        //}

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
            await destinationBlob.StartCopyAsync(sourceBlob, AccessCondition.GenerateIfExistsCondition(), AccessCondition.GenerateEmptyCondition(), null, null, token);
            while (destinationBlob.CopyState.Status != CopyStatus.Success)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.2), token);
                await destinationBlob.ExistsAsync();
            }
            await sourceBlob.DeleteAsync();
        }


        //public async Task MoveBlobAsync(MoveBlob source, MoveBlob destination, CancellationToken token)
        //{
        //    var sourceContainer = _client.GetContainerReference(source.ContainerName);
        //    var sourceBlob = sourceContainer.GetBlockBlobReference(source.FileName);
        //    var link = this.GenerateDownloadLink(sourceBlob.Uri, TimeSpan.FromDays(1));

        //    var destinationContainer = _client.GetContainerReference(destination.ContainerName);
        //    var destinationBlob = destinationContainer.GetBlockBlobReference(destination.FileName);

        //    await destinationBlob.StartCopyAsync(link);
        //    while (destinationBlob.CopyState.Status != CopyStatus.Success)
        //    {
        //        await Task.Delay(TimeSpan.FromSeconds(0.2), token);
        //        await destinationBlob.ExistsAsync();
        //    }
        //}

        public async Task DeleteDirectoryAsync(string id, CancellationToken token)
        {
            var directory = _blobDirectory.GetDirectoryReference(id);
            var blobs = await directory.ListBlobsSegmentedAsync(null);
            var l = new List<Task>();
            foreach (var blob in blobs.Results)
            {


                if (blob is CloudBlockBlob p)
                {

                    var t = p.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, AccessCondition.GenerateEmptyCondition(), new BlobRequestOptions(), new OperationContext(), token);
                    l.Add(t);
                }
            }

            await Task.WhenAll(l);

        }

        public async Task<IEnumerable<Uri>> FilesInDirectoryAsync(string directory, CancellationToken token)
        {
            var destinationDirectory = _blobDirectory.GetDirectoryReference(directory);
            var result = await destinationDirectory.ListBlobsSegmentedAsync(true, BlobListingDetails.None,
                1000, null, null, null, token);
            return result.Results.Select(s => s.Uri);
        }




        public async Task<IEnumerable<Uri>> FilesInDirectoryAsync(string prefix, string directory, CancellationToken token)
        {
            var path = $"{_container.RelativePath}/{directory}/{prefix}";
            var result = await _cloudContainer.ListBlobsSegmentedAsync(path, true,
                BlobListingDetails.None, 1000, null, null, null, token);
            return result.Results.Select(s => s.Uri);
        }

        public Uri GeneratePreviewLink(Uri blobUrl, TimeSpan expirationTime)
        {
            var blob = GetBlob(blobUrl);
            var signedUrl = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow + expirationTime

            });
            var url = new Uri(blob.Uri, signedUrl);
            return url;
        }

        public Uri GenerateDownloadLink(Uri blobUrl, TimeSpan expirationTime, string fileName = null)
        {
            var blob = GetBlob(blobUrl);
            var signedUrl = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow + expirationTime

            }, new SharedAccessBlobHeaders
            {
                ContentDisposition = "attachment; filename=\"" + WebUtility.UrlEncode(fileName ?? blob.Name) + "\""
            });


            var url = new Uri(blob.Uri, signedUrl);
            return url;
        }

    }

    public static class BlockBlobExtensions
    {
        public static Uri GetDownloadLink([NotNull] this CloudBlockBlob blob, TimeSpan expirationTime)
        {
            if (blob == null) throw new ArgumentNullException(nameof(blob));
            var signedUrl = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow + expirationTime

            });
            var url = new Uri(blob.Uri, signedUrl);
            return url;
        }
    }
}
