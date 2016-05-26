using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
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
    public class BlobProvider : IBlobProvider, ICloudBlockProvider, IBlobUpload
    {
        private const int CacheContainerItemAvailableInMinutes = 30;
        private const string LastAccessTimeMetaDataKey = "LastTimeAccess";
        private readonly string m_StorageCdnEndpoint = ConfigFetcher.Fetch("StorageCdnEndpoint");

        public const string AzureBlobContainer = "zboxfiles";
        public const string AzureCacheContainer = "zboxCahce";
        public const string AzureProductContainer = "zboxProductImages";
        public const string AzureProfilePicContainer = "zboxprofilepic";
        public const string AzurePreviewContainer = "preview";
        public const string AzureFaqContainer = "zboxhelp";
        public const string AzureQuizContainer = "zboxquestion";
        public const string AzureChatContainer = "zboxchat";


        internal const string AzureIdGeneratorContainer = "zboxIdGenerator";

        private readonly ILocalStorageProvider m_LocalStorageProvider;

        public BlobProvider(ILocalStorageProvider localStorageProvider)
        {
            m_LocalStorageProvider = localStorageProvider;
            InitStorage();
        }

        private void InitStorage()
        {
            m_BlobClient = StorageProvider.ZboxCloudStorage.CreateCloudBlobClient();
            StorageContainerUrl = m_BlobClient.BaseUri.AbsoluteUri;
        }



        //public async Task<string> UploadFromLink(byte[] data, string fileName)
        //{
        //    var container = BlobClient.GetContainerReference(AzureProductContainer.ToLower());
        //    var directory = container.GetDirectoryReference(Path.GetFileNameWithoutExtension(fileName));
        //    var storageCdnEndpoint = ConfigFetcher.Fetch("StorageCdnEndpoint");
        //    int index = 0;
        //    foreach (var blobInDirectory in directory.ListBlobs(blobListingDetails: BlobListingDetails.Metadata))
        //    {
        //        var x = blobInDirectory as CloudBlockBlob;
        //        if (x == null)
        //        {
        //            continue;
        //        }
        //        if (x.Properties.Length == data.LongLength)
        //        {
        //            var uriBuilder2 = new UriBuilder(x.Uri);
        //            if (string.IsNullOrEmpty(storageCdnEndpoint)) return uriBuilder2.Uri.AbsoluteUri;
        //            var storeCdnUri2 = new Uri(storageCdnEndpoint);
        //            uriBuilder2.Host = storeCdnUri2.Host;
        //            return uriBuilder2.Uri.AbsoluteUri;
        //        }
        //        index++;
        //    }
        //    var blob = directory.GetBlockBlobReference(index.ToString(CultureInfo.InvariantCulture) + Path.GetExtension(fileName));
        //    await blob.UploadFromByteArrayAsync(data, 0, data.Length);
        //    blob.Properties.ContentType = "image/jpeg";
        //    blob.Properties.CacheControl = "public, max-age=" + TimeConst.Year;
        //    await blob.SetPropertiesAsync();

        //    var uriBuilder = new UriBuilder(blob.Uri);
        //    if (string.IsNullOrEmpty(storageCdnEndpoint)) return uriBuilder.Uri.AbsoluteUri;
        //    var storeCdnUri = new Uri(storageCdnEndpoint);
        //    uriBuilder.Host = storeCdnUri.Host;

        //    return uriBuilder.Uri.AbsoluteUri;
        //}


        public string StorageContainerUrl { get; private set; }

        //public string ProfileContainerUrl { get; private set; }


        //public string GetBlobUrl(string blobName)
        //{
        //    return BlobContainerUrl + blobName;
        //}





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
        private CloudBlobContainer CacheContainer()
        {
            return BlobClient.GetContainerReference(AzureCacheContainer.ToLower());
        }
        private CloudBlockBlob CacheFile(string blobName)
        {

            return CacheContainer().GetBlockBlobReference(blobName);
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
            await blob.FetchAttributesAsync(token);
            return blob.Metadata;

        }

        public Task SaveMetaDataToBlobAsync(Uri blobUri, IDictionary<string, string> metaData, CancellationToken token)
        {
            if (metaData == null) throw new ArgumentNullException(nameof(metaData));
            var blob = GetBlob(blobUri);
            //var blob = GetFile(blobName);
            foreach (var item in metaData)
            {
                blob.Metadata[item.Key] = item.Value;
            }
            return blob.SetMetadataAsync(token);
        }



        public string GenerateSharedAccressReadPermissionInCache(string blobName, double expirationTimeInMinutes)
        {
            var blob = CacheFile(blobName);
            try
            {
                blob.Metadata.Add(LastAccessTimeMetaDataKey, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                blob.SetMetadata();
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == 404)
                {
                    return null;
                }
            }
            return GenerateSharedAccessPermission(blob, expirationTimeInMinutes, SharedAccessBlobPermissions.Read);
        }

        public string GenerateSharedAccressReadPermissionInCacheWithoutMeta(string blobName, double expirationTimeInMinutes)
        {
            var blob = CacheFile(blobName);
            return GenerateSharedAccessPermission(blob, expirationTimeInMinutes, SharedAccessBlobPermissions.Read);
        }



        public string GenerateSharedAccessReadPermissionInStorage(Uri blobUri, double expirationTimeInMinutes)
        {
            //if (blobUri == null) throw new ArgumentNullException(nameof(blobUri));
            //var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            //
            //
            //var blob = GetFile(blobName);
            var blob = GetBlob(blobUri);
            return GenerateSharedAccessPermission(blob, expirationTimeInMinutes, SharedAccessBlobPermissions.Read);
        }

        public Task<bool> ExistsAsync(Uri blobUri)
        {
            var blob = GetBlob(blobUri);
            return blob.ExistsAsync();
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





        #region Cache
        public async Task<string> UploadFileToCacheAsync(string blobName, Stream fileContent, string mimeType, bool fileGziped = false)
        {
            var cacheblob = CacheFile(blobName);
            fileContent.Seek(0, SeekOrigin.Begin);

            cacheblob.Properties.ContentType = mimeType;
            if (fileGziped)
            {
                cacheblob.Properties.ContentEncoding = "gzip";
            }

            cacheblob.Properties.CacheControl = "private, max-age=" + TimeConst.Minute * CacheContainerItemAvailableInMinutes;
            cacheblob.Metadata.Add(LastAccessTimeMetaDataKey, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            await cacheblob.UploadFromStreamAsync(fileContent);
            return GenerateSharedAccressReadPermissionInCache(blobName, CacheContainerItemAvailableInMinutes);

        }
        public async Task<string> UploadFileToCacheAsync(string blobName, byte[] fileContent, string mimeType, bool fileGziped = false)
        {
            //we don't need to dispose because we dispose it later in the function
            using (var ms = new MemoryStream(fileContent))
            {
                return await UploadFileToCacheAsync(blobName, ms, mimeType, fileGziped);
            }
        }

        //public bool CacheBlobExists(string blobName)
        //{
        //    var cacheblob = CacheFile(blobName);
        //    return cacheblob.Exists();
        //}



        #endregion

        #region UploadFile
        //public async Task<int> UploadFileBlockAsync(string blobName, Stream fileContent, int currentIndex)
        //{
        //
        //    var blob = GetFile(blobName);
        //    fileContent.Seek(0, SeekOrigin.Begin);
        //    await blob.PutBlockAsync(ToBase64(currentIndex), fileContent, null);
        //    return ++currentIndex;
        //}

        //public async Task<int> UploadFileBlockAsync(string blobName, string container, Stream fileContent,
        //    int currentIndex)
        //{
        //    var blob = GetFile(blobName, container);
        //    fileContent.Seek(0, SeekOrigin.Begin);
        //    await blob.PutBlockAsync(ToBase64(currentIndex), fileContent, null);
        //    return ++currentIndex;
        //}

        internal async Task UploadFileAsync(string blobName, string filePath, string mimeType)
        {
            var blob = GetFile(blobName);
            await blob.UploadFromFileAsync(filePath, FileMode.Open);
            //await blob.UploadFromFileAsync(filePath);
            blob.Properties.ContentType = mimeType;
            await blob.SetPropertiesAsync();
        }


        protected string ToBase64(int blockIndex)
        {
            var blockId = blockIndex.ToString("D10");
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(blockId));
        }
        //public async Task CommitBlockListAsync(string blobName, int currentIndex, string contentType)
        //{
        //    var blockList = Enumerable.Range(0, currentIndex).Select(ToBase64);
        //    var blob = GetFile(blobName);
        //    await blob.PutBlockListAsync(blockList);
        //    //blob.PutBlockList(fileUploadedDetails.BlockIds);
        //    blob.Properties.ContentType = contentType;
        //    blob.Properties.CacheControl = "private max-age=" + TimeConst.Week;
        //
        //    await blob.SetPropertiesAsync();
        //
        //}
        //public async Task CommitBlockListAsync(string blobName, string container, int currentIndex, string contentType)
        //{
        //    var blockList = Enumerable.Range(0, currentIndex).Select(ToBase64);
        //    var blob = GetFile(blobName, container);
        //    blob.Properties.ContentType = contentType;
        //    blob.Properties.CacheControl = "private max-age=" + TimeConst.Week;
        //    await blob.PutBlockListAsync(blockList);
        //}
        /// <summary>
        /// Upload file to storage from link
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <returns>The size of the file</returns>
        public async Task<long> UploadFromLinkAsync(string url, string fileName)
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
                    return blob.Properties.Length;
                }
            }
        }


        #endregion

        #region Profile

        public async Task<Uri> UploadProfilePictureAsync(string blobName, Stream fileContent)
        {
            if (blobName == null) throw new ArgumentNullException(nameof(blobName));
            if (fileContent == null) throw new ArgumentNullException(nameof(fileContent));
            var blob = ProfilePictureFile(blobName);
            if (await blob.ExistsAsync())
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
            await blob.UploadFromStreamAsync(fileContent);
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

            await blob.UploadFromStreamAsync(content);

            return TransferToCdnEndpoint(blob.Uri);

        }
        #endregion

        private string TransferToCdnEndpoint(Uri uri)
        {
            if (string.IsNullOrEmpty(m_StorageCdnEndpoint))
            {
                return uri.AbsoluteUri;
            }
            var path = uri.PathAndQuery;
            if (path.StartsWith("/"))
            {
                path = path.Remove(0, 1);

            }

            return VirtualPathUtility.AppendTrailingSlash(m_StorageCdnEndpoint) + path;
        }

        #region Thumbnail


        //public Task UploadFilePreviewAsync(Uri blobUri, Stream content, string mimeType, CancellationToken token = default(CancellationToken))
        //{
        //    content.Seek(0, SeekOrigin.Begin);
        //    var blob = BlobClient.GetContainerReference(AzurePreviewContainer).GetBlockBlobReference(blobName);
        //    blob.Properties.ContentType = mimeType;
        //    blob.Properties.CacheControl = "public, max-age=" + TimeConst.Year;
        //    return blob.UploadFromStreamAsync(content, token);
        //}

        #endregion

        #region files

        //public Stream DownloadFile(string fileName)
        //{
        //    var blob = GetFile(fileName);
        //    var ms = new MemoryStream();
        //    blob.DownloadToStream(ms);
        //    ms.Seek(0, SeekOrigin.Begin);
        //    return ms;
        //}



        //public async Task<Stream> DownloadFileAsync(string fileName, CancellationToken cancelToken)
        //{
        //    var blob = GetFile(fileName);
        //    var ms = new MemoryStream();
        //    await blob.DownloadToStreamAsync(ms, cancelToken);
        //    ms.Seek(0, SeekOrigin.Begin);
        //    return ms;
        //
        //}

        private CloudBlockBlob GetBlob(Uri blobUrl)
        {
           return new CloudBlockBlob(blobUrl, StorageProvider.ZboxCloudStorage.Credentials);
        }
        public async Task<Stream> DownloadFileAsync(Uri blobUrl, CancellationToken cancelToken)
        {
            var blob = GetBlob(blobUrl);
            var ms = new MemoryStream();
            await blob.DownloadToStreamAsync(ms, cancelToken);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;

        }


        public async Task<string> DownloadToLocalDiskAsync(Uri blobUri, CancellationToken cancelToken)
        {
            var blob = GetBlob(blobUri);
            var fileSystemLocation = Path.Combine(m_LocalStorageProvider.LocalStorageLocation, blob.Name);
            if (File.Exists(fileSystemLocation))
            {
                return fileSystemLocation;
            }
           
            try
            {
                await blob.DownloadToFileAsync(fileSystemLocation,
                    FileMode.Create, cancelToken);
                return fileSystemLocation;
            }
            catch (IOException)
            {
                m_LocalStorageProvider.DeleteOldFiles();
            }
            await blob.DownloadToFileAsync(fileSystemLocation,
                    FileMode.Create, cancelToken);
            return fileSystemLocation;

        }


        #endregion


        #region FAQRegion
        public async Task<Stream> GetFaqQuestionAsync()
        {
            try
            {
                var blob = m_BlobClient.GetContainerReference(AzureFaqContainer).GetBlockBlobReference("help.xml");
                return await blob.OpenReadAsync(); // we need async in here 
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
                return await blob.OpenReadAsync(); // we need async in here
            }
            catch (StorageException)
            {
                return null;
            }
        }

        #endregion









        public string GenerateWriteAccessPermissionToBlob(string blobName, string mimeType)
        {
            var blob = GetFile(blobName);
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

        public string GenerateReadAccessPermissionToBlob(string blobName)
        {
            var blob = GetFile(blobName);
            var queryString = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = DateTime.Now.AddMinutes(-2),
                SharedAccessExpiryTime = DateTime.Now.AddHours(2)
            });
            return blob.Uri + queryString;
        }




    }
}
