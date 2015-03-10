using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Blob;
using Zbang.Zbox.Infrastructure.Azure.Storage;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.StorageApp
{
    public class BlobProvider : IBlobProvider
    {
        public const string AzureThumbnailContainer = "zboxThumbnail";
        public const string AzureBlobContainer = "zboxfiles";
        public const string AzureCacheContainer = "zboxCahce";
        private const int CacheContainerItemAvailableInMinutes = 30;
        private const string LastAccessTimeMetaDataKey = "LastTimeAccess";

        public BlobProvider()
        {
            InitStorage();
        }
        private CloudBlobClient m_BlobClient;

        private readonly string m_StorageCdnEndpoint = ConfigFetcher.Fetch("StorageCdnEndpoint");

        internal CloudBlobClient BlobClient
        {
            get
            {
                if (m_BlobClient == null)
                {
                    InitStorage();
                }
                return m_BlobClient;
            }
        }

        private void InitStorage()
        {

            m_BlobClient = StorageProvider.ZboxCloudStorage.CreateCloudBlobClient();
            var serviceProperties = m_BlobClient.GetServiceProperties();
            serviceProperties.DefaultServiceVersion = "2014-02-14";
            m_BlobClient.SetServiceProperties(serviceProperties);

            BlobContainerUrl = VirtualPathUtility.AppendTrailingSlash(BlobClient.GetContainerReference(AzureBlobContainer.ToLower()).Uri.AbsoluteUri);


            if (string.IsNullOrEmpty(m_StorageCdnEndpoint))
            {
                ThumbnailContainerUrl =
                    VirtualPathUtility.AppendTrailingSlash(
                        BlobClient.GetContainerReference(AzureThumbnailContainer.ToLower()).Uri.AbsoluteUri);
            }
            else
            {
                ThumbnailContainerUrl =
                    VirtualPathUtility.AppendTrailingSlash(VirtualPathUtility.AppendTrailingSlash(m_StorageCdnEndpoint) +
                                                           AzureThumbnailContainer.ToLower());
            }

        }


        public string GenerateSharedAccressReadPermissionInCache(string blobName, double expirationTimeInMinutes)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UploadFileToCacheAsync(string blobName, byte[] fileContent, string mimeType, bool fileGziped = false)
        {
            using (var ms = new MemoryStream(fileContent))
            {
                return await UploadFileToCacheAsync(blobName, ms, mimeType, fileGziped);
            }
        }
        private CloudBlobContainer CacheContainer()
        {
            return BlobClient.GetContainerReference(AzureCacheContainer.ToLower());
        }
        private CloudBlockBlob CacheFile(string blobName)
        {

            return CacheContainer().GetBlockBlobReference(blobName);
        }
        public async Task<string> UploadFileToCacheAsync(string blobName, Stream fileContent, string mimeType, bool fileGziped = false)
        {
            var cacheblob = CacheFile(blobName);
            fileContent.Seek(0, SeekOrigin.Begin);
            //fileContent.Position = 0;

            cacheblob.Properties.ContentType = mimeType;
            if (fileGziped)
            {
                cacheblob.Properties.ContentEncoding = "gzip";
            }

            cacheblob.Properties.CacheControl = "private, max-age=" + TimeConsts.Minute * CacheContainerItemAvailableInMinutes;
            cacheblob.Metadata.Add(LastAccessTimeMetaDataKey, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            await cacheblob.UploadFromStreamAsync(fileContent);
            return GenerateSharedAccressReadPermissionInCache(blobName, CacheContainerItemAvailableInMinutes);
        }

        public Uri UploadProfilePicture(string blobName, byte[] fileContent)
        {
            throw new NotImplementedException();
        }

        public void UploadFileThumbnail(string fileName, Stream ms, string mimeType)
        {
            throw new NotImplementedException();
        }

        public Task UploadFileThumbnailAsync(string fileName, Stream ms, string mimeType)
        {
            throw new NotImplementedException();
        }

        public Task UploadFileThumbnailAsync(string fileName, Stream ms, string mimeType, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public bool CheckIfFileThumbnailExists(string blobName)
        {
            throw new NotImplementedException();
        }

        public Stream DownloadFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> DownloadFileAsync(string fileName)
        {
            return DownloadFileAsync(fileName, CancellationToken.None);
        }

        CloudBlockBlob GetFile(string blobName)
        {
            var blob = BlobClient.GetContainerReference(AzureBlobContainer.ToLower()).GetBlockBlobReference(blobName);

            return blob;
        }

        public async Task<Stream> DownloadFileAsync(string fileName, CancellationToken cancelToken)
        {
            CloudBlockBlob blob = GetFile(fileName);
            var ms = new MemoryStream();
            await blob.DownloadToStreamAsync(ms, cancelToken);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public Task<Stream> DownloadFileAsync2(string fileName, CancellationToken cancelToken)
        {
            CloudBlockBlob blob = GetFile(fileName);
            return blob.OpenReadAsync(cancelToken);
        }

        public Task<string> DownloadToFileAsync(string fileName, CancellationToken cancelToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> UploadFileBlockAsync(string blobName, Stream fileContent, int currentIndex)
        {
            throw new NotImplementedException();
        }

        public Task CommitBlockListAsync(string blobName, int currentIndex, string contentType)
        {
            throw new NotImplementedException();
        }

        public Task<long> UploadFromLinkAsync(string url, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task SaveMetaDataToBlobAsync(string blobName, IDictionary<string, string> metaData)
        {
            if (metaData == null) throw new ArgumentNullException("metaData");
            var blob = GetFile(blobName);
            foreach (var item in metaData)
            {
                blob.Metadata[item.Key] = item.Value;
            }
            return blob.SetMetadataAsync();
        }

        public async Task<IDictionary<string, string>> FetechBlobMetaDataAsync(string blobName)
        {
            var blob = GetFile(blobName);
            await blob.FetchAttributesAsync();
            return blob.Metadata;
        }

        public string GenerateSharedAccressReadPermissionInStorage(Uri blobUri, double expirationTimeInMinutes)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> GetFaqQuestion()
        {
            throw new NotImplementedException();
        }

        public string GenerateSharedAccressReadPermissionInCacheWithoutMeta(string blobName, double expirationTimeInMinutes)
        {
            throw new NotImplementedException();
        }

        public string ProfileContainerUrl
        {
            get { throw new NotImplementedException(); }
        }

        public string BlobContainerUrl { get; private set; }

        static string ThumbnailContainerUrl { get; set; }

        public string GetThumbnailUrl(string blobName)
        {
            return ThumbnailContainerUrl + blobName;
        }

        public string GetThumbnailLinkUrl()
        {
            throw new NotImplementedException();
        }

        public string GetBlobUrl(string blobName)
        {
            return BlobContainerUrl + blobName;
        }

        public void RenameBlob(string blobName, string newName, string newMimeType = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadQuizImage(Stream content, string mimeType, long boxId, string fileName)
        {
            throw new NotImplementedException();
        }

        public bool CacheBlobExists(string blobName)
        {
            throw new NotImplementedException();
        }
    }
}
