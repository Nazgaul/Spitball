using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Zbang.Zbox.Infrastructure.Azure.Storage;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Azure.Blob
{
    public class BlobProvider : IBlobProvider, ICloudBlockProvider
    {
        protected const string LastAccessTimeMetaDataKey = "LastTimeAccess";
        protected readonly string StorageCdnEndpoint = ConfigFetcher.Fetch("StorageCdnEndpoint");

        public const string AzureBlobContainer = "zboxfiles";
        public const string AzureCacheContainer = "zboxCahce";
        public const string AzureProductContainer = "zboxProductImages";
        public const string AzureProfilePicContainer = "zboxprofilepic";
        public const string AzurePreviewContainer = "preview";
        public const string AzureFaqContainer = "zboxhelp";
        public const string AzureQuizContainer = "zboxquestion";
        public const string AzureChatContainer = "zboxchat";


        internal const string AzureIdGeneratorContainer = "zboxIdGenerator";

        private readonly Lazy<ILocalStorageProvider> m_LocalStorageProvider;

        public BlobProvider(Lazy<ILocalStorageProvider> localStorageProvider)
        {
            m_LocalStorageProvider = localStorageProvider;
            InitStorage();
        }

        private void InitStorage()
        {
            m_BlobClient = StorageProvider.ZboxCloudStorage.CreateCloudBlobClient();
            StorageContainerUrl = m_BlobClient.BaseUri.AbsoluteUri;
        }

        public string StorageContainerUrl { get; private set; }

        private CloudBlobClient m_BlobClient;

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

        public string GetBlobNameFromUri(Uri blobUri)
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            return blobName;
        }

        private CloudBlockBlob ProfilePictureFile(string blobName)
        {
            return BlobClient.GetContainerReference(AzureProfilePicContainer.ToLower()).GetBlockBlobReference(blobName);
        }


        public CloudBlockBlob GetFile(string blobName)
        {
            var blob = BlobClient.GetContainerReference(AzureBlobContainer.ToLower()).GetBlockBlobReference(blobName);
            return blob;
        }

        public virtual CloudBlockBlob GetFile(string blobName, string containerName)
        {
            var container = BlobClient.GetContainerReference(containerName.ToLower());

            return container.GetBlockBlobReference(blobName);
        }

        public async Task<IDictionary<string, string>> FetchBlobMetaDataAsync(Uri blobUri, CancellationToken token)
        {
            var blob = GetBlob(blobUri);// GetFile(blobName);
            await blob.FetchAttributesAsync(token).ConfigureAwait(false);
            return blob.Metadata;

        }

        public Task SaveMetaDataToBlobAsync(Uri blobUri, IDictionary<string, string> metadata, CancellationToken token)
        {
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            var blob = GetBlob(blobUri);
            foreach (var item in metadata)
            {
                blob.Metadata[item.Key] = item.Value;
            }
            return blob.SetMetadataAsync(token);
        }



        public string GenerateSharedAccessReadPermissionInStorage(Uri blobUri, double expirationTimeInMinutes)
        {
            var blob = GetBlob(blobUri);
            return GenerateSharedAccessPermission(blob, expirationTimeInMinutes, SharedAccessBlobPermissions.Read);
        }

        private static string GenerateSharedAccessPermission(CloudBlockBlob blob, double expirationTimeInMinutes, SharedAccessBlobPermissions accessPermission)
        {

            var signedUrl = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                Permissions = accessPermission,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(expirationTimeInMinutes)
            });

            var url = new Uri(blob.Uri, signedUrl);
            return url.AbsoluteUri;
        }

        #region UploadFile

        internal async Task UploadFileAsync(string blobName, string filePath, string mimeType)
        {
            var blob = GetFile(blobName);
            await blob.UploadFromFileAsync(filePath).ConfigureAwait(false);
            blob.Properties.ContentType = mimeType;
            await blob.SetPropertiesAsync().ConfigureAwait(false);
        }


        protected static string ToBase64(int blockIndex)
        {
            var blockId = blockIndex.ToString("D10");
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(blockId));
        }


        #endregion

        #region Profile

        public async Task<Uri> UploadProfilePictureAsync(string blobName, Stream fileContent)
        {
            if (blobName == null) throw new ArgumentNullException(nameof(blobName));
            if (fileContent == null) throw new ArgumentNullException(nameof(fileContent));
            var blob = ProfilePictureFile(blobName);
            if (await blob.ExistsAsync().ConfigureAwait(false))
            {
                if (blob.Properties.Length == fileContent.Length)
                {
                    return blob.Uri;
                }
            }
            // using (var ms = new MemoryStream(fileContent, false))
            //{
            fileContent.Seek(0, SeekOrigin.Begin);
            blob.Properties.ContentType = "image/jpeg";
            blob.Properties.CacheControl = "public, max-age=" + TimeConst.Year;
            await blob.UploadFromStreamAsync(fileContent).ConfigureAwait(false);
            //}
            return blob.Uri;
        }


        #endregion

        #region Quiz

        public async Task<string> UploadQuizImageAsync(Stream content, string mimeType, long boxId, string fileName)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (!mimeType.ToLower().Contains("image"))
            {
                throw new ArgumentException("this is not an image. mime type: " + mimeType);
            }
            var name = Guid.NewGuid();
            var container = BlobClient.GetContainerReference(AzureQuizContainer.ToLower());
            var directory = container.GetDirectoryReference(boxId.ToString(CultureInfo.InvariantCulture));
            var blob = directory.GetBlockBlobReference(name + Path.GetExtension(fileName));

            blob.Properties.ContentType = mimeType;
            blob.Properties.CacheControl = "public, max-age=" + TimeConst.Year;

            await blob.UploadFromStreamAsync(content).ConfigureAwait(false);

            return TransferToCdnEndpoint(blob.Uri);

        }
        #endregion

        private string TransferToCdnEndpoint(Uri uri)
        {
            if (string.IsNullOrEmpty(StorageCdnEndpoint))
            {
                return uri.AbsoluteUri;
            }
            var path = uri.PathAndQuery;
            if (path.StartsWith("/"))
            {
                path = path.Remove(0, 1);

            }

            return VirtualPathUtility.AppendTrailingSlash(StorageCdnEndpoint) + path;
        }


        #region files

        private static CloudBlockBlob GetBlob(Uri blobUrl)
        {
           return new CloudBlockBlob(blobUrl, StorageProvider.ZboxCloudStorage.Credentials);
        }
        public async Task<Stream> DownloadFileAsync(Uri blobUrl, CancellationToken cancelToken)
        {
            var blob = GetBlob(blobUrl);
            var ms = new MemoryStream();
            await blob.DownloadToStreamAsync(ms, cancelToken).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;

        }

        public Task<Stream> OpenBlobStreamAsync(Uri blobUri, CancellationToken cancelToken)
        {
            var blob = GetBlob(blobUri);
            return blob.OpenReadAsync(cancelToken);
        }
        public async Task<string> DownloadToLocalDiskAsync(Uri blobUri, CancellationToken cancelToken)
        {
            var blob = GetBlob(blobUri);
            var fileSystemLocation = Path.Combine(m_LocalStorageProvider.Value.LocalStorageLocation, blob.Name);
            if (File.Exists(fileSystemLocation))
            {
                return fileSystemLocation;
            }
            try
            {
                await blob.DownloadToFileAsync(fileSystemLocation,
                    FileMode.Create, cancelToken).ConfigureAwait(false);
                return fileSystemLocation;
            }
            catch (IOException)
            {
                m_LocalStorageProvider.Value.DeleteOldFiles();
            }
            await blob.DownloadToFileAsync(fileSystemLocation,
                FileMode.Create, cancelToken).ConfigureAwait(false);
            return fileSystemLocation;

        }


        #endregion


        #region FAQRegion
        public async Task<Stream> GetFaqQuestionAsync()
        {
            try
            {
                var blob = m_BlobClient.GetContainerReference(AzureFaqContainer).GetBlockBlobReference("help.xml");
                return await blob.OpenReadAsync().ConfigureAwait(false); // we need async in here
            }
            catch (StorageException)
            {
                return null;
            }
        }
        public async Task<Stream> GetJobsXmlAsync()
        {
            try
            {
                var blob = m_BlobClient.GetContainerReference(AzureFaqContainer).GetBlockBlobReference("jobs2.xml");
                return await blob.OpenReadAsync().ConfigureAwait(false); // we need async in here
            }
            catch (StorageException)
            {
                return null;
            }
        }

        #endregion
    }
}
