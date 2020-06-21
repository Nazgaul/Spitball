using Cloudents.Core.Extension;
using Cloudents.Core.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Storage
{
    public class BlobProviderContainer : IBlobProvider,
        IChatDirectoryBlobProvider
    {
        private readonly IConfigurationKeys _storageProvider;

        protected readonly BlobContainerClient _cloudContainer;
        private readonly StorageContainer _container;
        private readonly BlobServiceClient _client;
        private const string CdnHostEndpoint = "az32006.vo.msecnd.net";


        public BlobProviderContainer(IConfigurationKeys storageProvider)
        {
            _storageProvider = storageProvider;
            //_client = storageProvider.GetBlobClient();

            _client = new BlobServiceClient(storageProvider.Storage);
            //blobServiceClient.GetBlobContainerClient()
            //_client = storageProvider.GetBlobClient();
            //_connectionString = connectionString.Storage;
            //var x = new BlockBlobClient();();
        }

        public BlobProviderContainer(IConfigurationKeys storageProvider, StorageContainer container)
        {
            _client = new BlobServiceClient(storageProvider.Storage);
            _storageProvider = storageProvider;
            _cloudContainer = _client.GetBlobContainerClient(container.Name.ToLowerInvariant());

            _container = container;
            //var x = _cloudContainer.GetBlockBlobClient();

            //BlobDirectory = _cloudContainer.GetDirectoryReference(container.RelativePath);
        }


        private Dictionary<string, string> ParseConnectionString()
        {
            var settings = new Dictionary<string, string>();
            var splitted = _storageProvider.Storage.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var nameValue in splitted)
            {
                var splittedNameValue = nameValue.Split(new char[] { '=' }, 2);

                //if (splittedNameValue.Length != 2)
                //{
                //    error("Settings must be of the form \"name=value\".");
                //    return null;
                //}

                //if (settings.ContainsKey(splittedNameValue[0]))
                //{
                //    error(string.Format(CultureInfo.InvariantCulture, "Duplicate setting '{0}' found.", splittedNameValue[0]));
                //    return null;
                //}

                settings.Add(splittedNameValue[0], splittedNameValue[1]);
            }

            return settings;
        }


        public Uri GetBlobUrl(string blobName, bool cdn = false)
        {
            var blob = GetBlob(blobName);
            //var blob = BlobDirectory.GetBlockBlobReference(blobName);
            var uri = blob.Uri;
            if (cdn)
            {
                uri = uri.ChangeHost(CdnHostEndpoint);
            }
            return uri;
        }

        private BlobClient GetBlob(Uri blobUrl)
        {

            return new BlobClient(blobUrl);//,new StorageSharedKeyCredential(_client.AccountName,"zzz"));
        }

        protected BlockBlobClient GetBlob(string blobName)
        {
            if (string.IsNullOrEmpty(_container.RelativePath))
            {
                return _cloudContainer.GetBlockBlobClient(blobName);

            }
            return _cloudContainer.GetBlockBlobClient($"{_container.RelativePath}/{blobName}");

            //return BlobDirectory.GetBlockBlobReference(blobName);
        }


        public virtual Task UploadStreamAsync(string blobName, Stream fileContent,
            string? mimeType = null, TimeSpan? cacheControlSeconds = null, CancellationToken token = default)
        {
            var blob = GetBlob(blobName);
            if (fileContent.CanSeek)
            {
                fileContent.Seek(0, SeekOrigin.Begin);
                //throw new ArgumentException("stream should need to be able to seek");
            }

            var headers = new BlobHttpHeaders();
            if (mimeType != null)
            {
                headers.ContentType = mimeType;
            }

            //if (fileGziped)
            //{
            //    blob.Properties.ContentEncoding = "gzip";
            //}

            if (cacheControlSeconds.HasValue)
            {
                headers.CacheControl = "private, max-age=" + cacheControlSeconds.Value.TotalSeconds;
            }

            return blob.UploadAsync(fileContent, headers, cancellationToken: token);
        }

        public Task UploadBlockFileAsync(string blobName, Stream fileContent, int index, CancellationToken token)
        {
            var blob = GetBlob(blobName);

            fileContent.Seek(0, SeekOrigin.Begin);
            return blob.StageBlockAsync(ToBase64(index), fileContent, cancellationToken: token);
            //return blob.PutBlockAsync(ToBase64(index), fileContent, null, null, new BlobRequestOptions
            //{
            //    StoreBlobContentMD5 = true
            //}, null, token);
        }


        //public Task CommitBlockListAsync(string blobName, string mimeType, IList<int> indexes, CancellationToken token)
        //{
        //    return CommitBlockListAsync(blobName, mimeType, null, indexes, null, token);
        //}



        public Task CommitBlockListAsync(string blobName, string mimeType, string? originalFileName, IEnumerable<int> indexes, TimeSpan? cacheControlTime = null, CancellationToken token = default)
        {
            var blob = GetBlob(blobName);
            var headers = new BlobHttpHeaders();
            if (cacheControlTime.HasValue)
            {
                headers.CacheControl = "max-age=" + cacheControlTime.Value.TotalSeconds;
            }

            headers.ContentType = mimeType;
            
            //if (!string.IsNullOrEmpty(originalFileName))
            //{
            //    blob.Metadata["fileName"] = originalFileName;
            //}

            return blob.CommitBlockListAsync(indexes.Select(ToBase64), headers, new Dictionary<string, string>()
            {
                ["fileName"] = originalFileName ?? ""
            }, cancellationToken: token);

            //return blob.PutBlockListAsync(indexes.Select(ToBase64), AccessCondition.GenerateEmptyCondition(), new BlobRequestOptions()
            //{
            //    StoreBlobContentMD5 = true
            //}, null, token);
        }

        private static string ToBase64(int blockIndex)
        {
            var blockId = blockIndex.ToString("D10");
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(blockId));
        }

        //public Task<bool> ExistsAsync(string blobName, CancellationToken token)
        //{
        //    var blob = GetBlob(blobName);
        //    return blob.ExistsAsync();
        //}

        public async Task MoveAsync(string blobName, string destinationContainerName, CancellationToken token)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentException("message", nameof(blobName));
            }
            var destinationBlob = _cloudContainer.GetBlockBlobClient($"{_container.RelativePath}/{destinationContainerName}/{blobName}");
            var sourceBlob = GetBlob(blobName);


           var status =  await destinationBlob.StartCopyFromUriAsync(sourceBlob.Uri, cancellationToken: token);


           while (!status.HasCompleted)
           {
               await Task.Delay(TimeSpan.FromSeconds(0.2), token);
               await status.UpdateStatusAsync(token);
           }

           await sourceBlob.DeleteAsync(cancellationToken: token);
            //var destinationDirectory = BlobDirectory.GetDirectoryReference(destinationContainerName);

            //sourceBlob.StartCopyFromUriAsync()


            //var destinationBlob = destinationDirectory.GetBlockBlobReference(blobName);
            //await destinationBlob.StartCopyAsync(sourceBlob, AccessCondition.GenerateIfExistsCondition(), AccessCondition.GenerateEmptyCondition(), new BlobRequestOptions()
            //{
            //    StoreBlobContentMD5 = true
            //}, null, token);
            //while (destinationBlob.CopyState.Status != CopyStatus.Success)
            //{
            //    await Task.Delay(TimeSpan.FromSeconds(0.2), token);
            //    await destinationBlob.ExistsAsync();
            //}
            //await sourceBlob.DeleteAsync();
        }

        public async Task DeleteDirectoryAsync(string id, CancellationToken token)
        {
            //TODO we can do it in batch
            var l = new List<Task>();
            await foreach (var page in _cloudContainer
                .GetBlobsByHierarchyAsync(prefix: $"{_container.RelativePath}/{id}").AsPages().WithCancellation(token))
            {
                foreach (var blobHierarchyItem in page.Values)
                {
                    if (blobHierarchyItem.IsBlob)
                    {
                        var blobClient = _cloudContainer.GetBlobClient(blobHierarchyItem.Blob.Name);
                        var t = blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: token);
                        l.Add(t);
                    }
                }
            }

         //   var x = _client.GetBlobBatchClient();


            await Task.WhenAll(l);

        }

        //public async Task UnDeleteDirectoryAsync(string id, CancellationToken token)
        //{
        //    var directory = BlobDirectory.GetDirectoryReference(id);
        //    var blobs = await directory.ListBlobsSegmentedAsync(useFlatBlobListing: true, blobListingDetails: BlobListingDetails.Deleted, null, new BlobContinuationToken(), new BlobRequestOptions(),
        //        new OperationContext(), token);
        //    var l = new List<Task>();
        //    foreach (var blob in blobs.Results)
        //    {


        //        if (blob is CloudBlockBlob p)
        //        {

        //            var t = p.UndeleteAsync(AccessCondition.GenerateEmptyCondition(), new BlobRequestOptions(), new OperationContext(), token);
        //            l.Add(t);
        //        }
        //    }

        //    await Task.WhenAll(l);
        //}

        //public async Task<IEnumerable<Uri>> FilesInDirectoryAsync(string directory, CancellationToken token)
        //{
        //    var destinationDirectory = BlobDirectory.GetDirectoryReference(directory);
        //    var result = await destinationDirectory.ListBlobsSegmentedAsync(true, BlobListingDetails.None,
        //        1000, null, null, null, token);
        //    return result.Results.Select(s => s.Uri);
        //}






        public async IAsyncEnumerable<Uri> FilesInDirectoryAsync(string prefix, string directory, CancellationToken token)
        {
            var path = $"{_container.RelativePath}/{directory}/{prefix}";

            await foreach (var page in _cloudContainer.GetBlobsAsync(prefix: path).AsPages(pageSizeHint: 1000).WithCancellation(token))
            {
                foreach (var item in page.Values)
                {
                    var blobClient = _cloudContainer.GetBlobClient(item.Name);
                    yield return blobClient.Uri;
                }
            }
            //var result = await _cloudContainer.ListBlobsSegmentedAsync(path, true,
            //    BlobListingDetails.None, 1000, null, null, null, token);
            //return result.Results.Select(s => s.Uri);
        }

        public Task<Uri> GeneratePreviewLinkAsync(Uri blobUrl, TimeSpan expirationTime)
        {
            var blob = GetBlob(blobUrl);
            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blob.BlobContainerName,
                BlobName = blob.Name,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
                ExpiresOn = DateTimeOffset.UtcNow + expirationTime
            };
            
            var storageSharedKeyCredential = new StorageSharedKeyCredential(_client.AccountName, ParseConnectionString()["AccountKey"]);
            //UserDelegationKey key = await _client.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow,
            //    DateTimeOffset.UtcNow.AddHours(1));

            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            var sasToken = sasBuilder.ToSasQueryParameters(storageSharedKeyCredential).ToString();

            var uriBuilder = new UriBuilder(blob.Uri)
            {
                Query = sasToken
            };

            return Task.FromResult(uriBuilder.Uri);
        }

        public Task<Uri> GenerateDownloadLinkAsync(Uri blobUrl, TimeSpan expirationTime, string? fileName = null)
        {

            var blob = GetBlob(blobUrl);
            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blob.BlobContainerName,
                BlobName = blob.Name,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
                ExpiresOn = DateTimeOffset.UtcNow + expirationTime,
                ContentDisposition = "attachment; filename=\"" + WebUtility.UrlEncode(fileName ?? blob.Name) + "\""
            };

            var storageSharedKeyCredential = new StorageSharedKeyCredential(_client.AccountName, ParseConnectionString()["AccountKey"]);

            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            var sasToken = sasBuilder.ToSasQueryParameters(storageSharedKeyCredential).ToString();

            var uriBuilder = new UriBuilder(blob.Uri)
            {
                Query = sasToken
            };

            return Task.FromResult(uriBuilder.Uri);
        }

    }
}
