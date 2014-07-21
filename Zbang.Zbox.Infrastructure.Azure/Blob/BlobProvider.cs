using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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
    public class BlobProvider : IBlobProvider, IBlobProductProvider
    {
        private const int CacheContainerItemAvaibleInMinutes = 30;
        private const string LastAccessTimeMetaDataKey = "LastTimeAccess";

        //public const string BlobMetadataUseridKey = "Userid";
        public const string AzureBlobContainer = "zboxfiles";
        public const string AzureCacheContainer = "zboxCahce";
        public const string AzureProductContainer = "zboxProductImages";
        public const string AzureProfilePicContainer = "zboxprofilepic";
        public const string AzureThumbnailContainer = "zboxThumbnail";
        public const string AzureFaQContainer = "zboxhelp";


        internal const string AzureIdGeneratorContainer = "zboxIdGenerator";

        public BlobProvider()
        {
            InitStorage();
        }
        private void InitStorage()
        {

            m_BlobClient = StorageProvider.ZboxCloudStorage.CreateCloudBlobClient();
            //System.Net.ServicePointManager.DefaultConnectionLimit = 20;
            //m_BlobClient.ParallelOperationThreadCount = 20;
            //CreateBlobStorages(m_BlobClient);

            BlobContainerUrl = VirtualPathUtility.AppendTrailingSlash(BlobClient.GetContainerReference(AzureBlobContainer.ToLower()).Uri.AbsoluteUri);
            string storageCdnEndpoint = ConfigFetcher.Fetch("StorageCdnEndpoint");

            if (string.IsNullOrEmpty(storageCdnEndpoint))
            {
                ThumbnailContainerUrl =
                    VirtualPathUtility.AppendTrailingSlash(
                        BlobClient.GetContainerReference(AzureThumbnailContainer.ToLower()).Uri.AbsoluteUri);
                ProfileContainerUrl = VirtualPathUtility.AppendTrailingSlash(BlobClient.GetContainerReference(AzureProfilePicContainer).Uri.AbsoluteUri);
            }
            else
            {
                ThumbnailContainerUrl =
                    VirtualPathUtility.AppendTrailingSlash(VirtualPathUtility.AppendTrailingSlash(storageCdnEndpoint) +
                                                           AzureThumbnailContainer.ToLower());
                ProfileContainerUrl =
                    VirtualPathUtility.AppendTrailingSlash(VirtualPathUtility.AppendTrailingSlash(storageCdnEndpoint) +
                                                           AzureProfilePicContainer.ToLower());
            }

        }

        /// <summary>
        /// Upload Image of product to product container storage
        /// </summary>
        /// <param name="data">image content</param>
        /// <param name="fileName">image fileName</param>
        /// <returns>link to that image</returns>
        public async Task<string> UploadFromLink(byte[] data, string fileName)
        {
            var container = BlobClient.GetContainerReference(AzureProductContainer.ToLower());
            var blob = container.GetBlockBlobReference(fileName);

            var uriBuilder = new UriBuilder(blob.Uri);
            string storageCdnEndpoint = ConfigFetcher.Fetch("StorageCdnEndpoint");
            if (!string.IsNullOrEmpty(storageCdnEndpoint))
            {
                uriBuilder.Host = storageCdnEndpoint;
            }


            if (blob.Exists())
            {
                return uriBuilder.Uri.AbsoluteUri;
            }

            await blob.UploadFromByteArrayAsync(data, 0, data.Length);
            blob.Properties.ContentType = "image/jpeg";
            blob.Properties.CacheControl = "public, max-age=" + TimeConsts.Year;
            await blob.SetPropertiesAsync();

            return uriBuilder.Uri.AbsoluteUri;
        }

        static string ThumbnailContainerUrl { get; set; }
        public string BlobContainerUrl { get; private set; }

        public string ProfileContainerUrl { get; private set; }


        public string GetBlobUrl(string blobName)
        {
            return BlobContainerUrl + blobName;
        }

        public string GetThumbnailUrl(string blobName)
        {
            return ThumbnailContainerUrl + blobName;
        }

        string LinkThumbnailUrl
        {
            get { return GetThumbnailUrl("linkv2.jpg"); }
        }
        public string GetThumbnailLinkUrl()
        {
            return LinkThumbnailUrl;
        }

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
        private CloudBlockBlob ThumbnailFile(string blobName)
        {
            return BlobClient.GetContainerReference(AzureThumbnailContainer.ToLower()).GetBlockBlobReference(blobName);
        }

        public CloudBlockBlob GetFile(string blobName)
        {
            var blob = BlobClient.GetContainerReference(AzureBlobContainer.ToLower()).GetBlockBlobReference(blobName);

            return blob;
        }

        public async Task<IDictionary<string, string>> FetechBlobMetaDataAsync(string blobName)
        {
            var blob = GetFile(blobName);
            await blob.FetchAttributesAsync();
            return blob.Metadata;

        }

        public Task SaveMetaDataToBlobAsync(string blobName, IDictionary<string, string> metaData)
        {
            if (metaData == null) throw new ArgumentNullException("metaData");
            var blob = GetFile(blobName);
            foreach (var item in metaData)
            {
                //   System.Convert.ToBase64String(
                blob.Metadata.Add(item.Key, item.Value);
            }
            return blob.SetMetadataAsync();
        }


        public string GenerateSharedAccressReadPermissionInCache(string blobName, double experationTimeInMinutes)
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
            return GenerateSharedAccessPermission(blob, experationTimeInMinutes, SharedAccessBlobPermissions.Read);
        }

        public string GenerateSharedAccressReadPermissionInCacheWithoutMeta(string blobName, double experationTimeInMinutes)
        {
            var blob = CacheFile(blobName);
            return GenerateSharedAccessPermission(blob, experationTimeInMinutes, SharedAccessBlobPermissions.Read);
        }



        public string GenerateSharedAccressReadPermissionInStorage(Uri blobUri, double experationTimeInMinutes)
        {
            if (blobUri == null) throw new ArgumentNullException("blobUri");
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];


            var blob = GetFile(blobName);
            return GenerateSharedAccessPermission(blob, experationTimeInMinutes, SharedAccessBlobPermissions.Read);
        }



        private string GenerateSharedAccessPermission(CloudBlockBlob blob, double experationTimeInMinutes, SharedAccessBlobPermissions accessPermission)
        {

            var signedurl = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                Permissions = accessPermission,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(experationTimeInMinutes)
            });

            var url = new Uri(blob.Uri, signedurl);
            return url.AbsoluteUri;
        }


        #region MimeType
        //public string FetchBlobMimeType(string fileName)
        //{
        //    var blob = GetFile(fileName);
        //    blob.FetchAttributes();
        //    var mimeType = blob.Properties.ContentType;
        //    if (HaveNoCategory(mimeType))
        //    {
        //        m_ExtensionToMimeType.TryGetValue(Path.GetExtension(blob.Name.ToLower()), out mimeType);
        //        if (string.IsNullOrEmpty(mimeType))
        //        {
        //            mimeType = "application/octet-stream";
        //            TraceLog.WriteError("Trying to get mime type to " + blob.Name.ToLower());
        //        }
        //        blob.Properties.ContentType = mimeType;
        //        blob.SetProperties();
        //    }
        //    return mimeType.ToLower();

        //}

        //private bool HaveNoCategory(string mimeType)
        //{
        //    return string.IsNullOrWhiteSpace(mimeType) || mimeType == "application/octet-stream";
        //}


        //private readonly Dictionary<string, string> m_ExtensionToMimeType = new Dictionary<string, string>
        //                                                     {
        //    //image
        //    {".gif","image/gif"},
        //    {".jpg","image/jpeg"},
        //    {".png","image/png"},
        //    {".tiff","image/tiff"},
        //    {".bmp","image/bmp"},
        //    {".psd","image/photoshop"},
        //    //audio
        //    {".mp3","audio/mpeg"},
        //    {".wav","audio/x-wav"},
        //    {".dvf","audio/x-dvf"},
        //    //video
        //    {".mp4","video/mp4"},
        //    {".mpeg","video/mpeg"},
        //    {".ogg","video/ogg"},
        //    {".webm","video/webm"},
        //    {".avi","video/avi"},
        //    {".wmv","video/x-ms-wmv"},
        //    {".mov","video/quicktime"},
        //    {".wma","video/x-ms-asf"},
        //    //pdf
        //    {".pdf","application/pdf"},
        //    //text
        //    {".abc","text/vnd.abc"},
        //    {".acgi","text/html"},
        //    {".htm","text/html"},
        //    {".html","text/html"},
        //    {".aip","text/x-audiosoft-intra"},
        //    {".asm","text/x-asm"},
        //    {".asp","text/asp"},
        //    {".c","text/plain"},
        //    {".h","text/plain"},
        //    {".cs","text/plain"},
        //    {".c++","text/plain"},
        //    {".cc","text/plain"},
        //    {".conf","text/plain"},
        //    {".cpp","text/x-c"},
        //    {".csh","text/x-script.csh"},
        //    {".css","text/css"},
        //    {".cxx","text/plain"},
        //    {".txt","text/plain"},
        //    {".java","text/plain"},
        //    {".rtf","text/rtf"},

        //    //office extension
        //    {".doc","application/msword"},
        //    {".xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        //    {".xltx","application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        //    {".potx","application/vnd.openxmlformats-officedocument.presentationml.template"},
        //    {".ppsx","application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        //    {".pptx","application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        //    {".sldx","application/vnd.openxmlformats-officedocument.presentationml.slide"},
        //    {".docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        //    {".dotx","application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        //    {".xlam","application/vnd.ms-excel.addin.macroEnabled.12"},
        //    {".xlsb","application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        //    {".mdb","application/vnd.ms-access"},
        //    {".ppt","application/mspowerpoint"},

        //    //compress
        //    {".rar","application/x-rar-compressed"},
        //    {".zip","application/zip"},

        //    //others
        //    {".ai","application/postscript"}




        //};
        #endregion


        #region Cache
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

            cacheblob.Properties.CacheControl = "private, max-age=" + TimeConsts.Minute * CacheContainerItemAvaibleInMinutes;
            cacheblob.Metadata.Add(LastAccessTimeMetaDataKey, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            await cacheblob.UploadFromStreamAsync(fileContent);
            fileContent.Dispose();

            //await Task.Factory.FromAsync(cacheblob.BeginUploadFromStream, (async) =>
            //{
            //    fileContent.Dispose();
            //    cacheblob.EndUploadFromStream(async);
            //},
            //    //  cacheblob.EndUploadFromStream,
            //    fileContent, null);

            //cacheblob.UploadFromStream(fileContent);

            return GenerateSharedAccressReadPermissionInCache(blobName, CacheContainerItemAvaibleInMinutes);// GenerateSharedAccessReadPermissionBlobFiles(cacheblob, CacheContainerItemAvaibleInMinutes);

        }
        public Task<string> UploadFileToCacheAsync(string blobName, byte[] fileContent, string mimeType, bool fileGziped = false)
        {
            using (var ms = new MemoryStream(fileContent))
            {
                //using (var ms = new MemoryStream(fileContent))
                //{
                return UploadFileToCacheAsync(blobName, ms, mimeType, fileGziped);
            }
            //}
        }

        //public bool CheckIfFileExistsInCache(string blobName)
        //{
        //    var blob = CacheFile(blobName);

        //    return blob.Exists();
        //}
        //public string GetFileUrlInCahce(string blobName)
        //{
        //    var blob = CacheFile(blobName);

        //    blob.Metadata.Add(LastAccessTimeMetaDataKey, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
        //    blob.SetMetadataAsync();
        //    //blob.BeginSetMetadata((async) =>
        //    //{

        //    //}, null);
        //    return GenerateSharedAccessReadPermissionBlobFiles(blob, CacheContainerItemAvaibleInMinutes);
        //}
        //public void CheckIfFileExists()
        //{
        //    const int daysObjectInCache = 30;

        //    var blobContainer = CacheContainer();
        //    var blobList = blobContainer.ListBlobs(null, true, BlobListingDetails.Metadata);
        //    //var blobList = blobContainer.ListBlobs(new BlobRequestOptions { BlobListingDetails = BlobListingDetails.Metadata, UseFlatBlobListing = true });
        //    foreach (var blobelement in blobList)
        //    {

        //        var lastDateAccess = DateTime.UtcNow.AddMonths(-1);
        //        var blob = blobContainer.GetBlockBlobReference(blobelement.Uri.ToString());
        //        blob.FetchAttributes();
        //        string lastAccessDate = blob.Metadata[LastAccessTimeMetaDataKey];
        //        DateTime.TryParse(lastAccessDate, out lastDateAccess);

        //        if (DateTime.UtcNow - lastDateAccess > TimeSpan.FromDays(daysObjectInCache))
        //        {
        //            blob.Delete();
        //        }
        //    }
        //}
        #endregion

        #region UploadFile
        public async Task<int> UploadFileBlockAsync(string blobName, Stream fileContent, int currentIndex)
        {
            var blob = GetFile(blobName);
            fileContent.Seek(0, SeekOrigin.Begin);
            await blob.PutBlockAsync(ToBase64(currentIndex), fileContent, null);
            return ++currentIndex;
        }
        internal async Task UploadFileAsync(string blobName, string filePath, string mimeType)
        {
            var blob = GetFile(blobName);
            await blob.UploadFromFileAsync(filePath, FileMode.Open);
            blob.Properties.ContentType = mimeType;
            await blob.SetPropertiesAsync();
        }
        //public async Task RenameBlobAsync(string blobName, string newName, string newMimeType = null)
        //{
        //    var blob = GetFile(blobName);
        //    var newBlob = GetFile(newName);
        //    await newBlob.StartCopyFromBlobAsync(blob);
        //    newBlob.Properties.ContentType = newMimeType ?? blob.Properties.ContentType;
        //    var t1 = newBlob.SetPropertiesAsync();
        //    var t2 = blob.DeleteAsync();
        //    await Task.WhenAll(t1, t2);
        //}
        private string ToBase64(int blockIndex)
        {
            var blockId = blockIndex.ToString("D10");
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(blockId));
        }
        public async Task CommitBlockListAsync(string blobName, int currentIndex, string contentType)
        {
            var blockList = Enumerable.Range(0, currentIndex).Select(ToBase64);
            var blob = GetFile(blobName);
            await blob.PutBlockListAsync(blockList);
            //blob.PutBlockList(fileUploadedDetails.BlockIds);
            blob.Properties.ContentType = contentType;
            blob.Properties.CacheControl = "private max-age=" + TimeConsts.Week;

            await blob.SetPropertiesAsync();

        }
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
                    blob.Properties.CacheControl = "private max-age=" + TimeConsts.Week;
                    await blob.SetPropertiesAsync();
                    return blob.Properties.Length;
                }
            }
        }


        #endregion

        #region Profile

        public Uri UploadProfilePicture(string blobName, byte[] fileContent)
        {
            if (blobName == null) throw new ArgumentNullException("blobName");
            if (fileContent == null) throw new ArgumentNullException("fileContent");
            var blob = ProfilePictureFile(blobName);
            if (blob.Exists())
            {
                if (blob.Properties.Length == fileContent.Length)
                {
                    return blob.Uri;
                }
            }
            using (var ms = new MemoryStream(fileContent, false))
            {
                blob.UploadFromStream(ms);
            }
            blob.Properties.ContentType = "image/jpeg";
            blob.Properties.CacheControl = "public, max-age=" + TimeConsts.Year;

            blob.SetProperties();

            return blob.Uri;
        }


        #endregion

        #region Thumbnail
        public void UploadFileThumbnail(string fileName, Stream ms, string mimeType)
        {
            if (ms == null) throw new ArgumentNullException("ms");
            ms.Seek(0, SeekOrigin.Begin);
            var thumbnailBlob = ThumbnailFile(fileName);
            thumbnailBlob.Properties.ContentType = mimeType;
            thumbnailBlob.Properties.CacheControl = "public, max-age=" + TimeConsts.Year;

            thumbnailBlob.UploadFromStream(ms);//ToDo: due to problem of small images the memory stream can be closed there fore remove back to sync process
            thumbnailBlob.SetProperties();
        }

        public Task UploadFileThumbnailAsync(string fileName, Stream ms, string mimeType)
        {
            if (ms == null) throw new ArgumentNullException("ms");
            ms.Seek(0, SeekOrigin.Begin);
            var thumbnailBlob = ThumbnailFile(fileName);
            thumbnailBlob.Properties.ContentType = mimeType;
            thumbnailBlob.Properties.CacheControl = "public, max-age=" + TimeConsts.Year;

            return thumbnailBlob.UploadFromStreamAsync(ms);//ToDo: due to problem of small images the memory stream can be closed there fore remove back to sync process
            //thumbnailBlob.SetProperties();
        }
        public bool CheckIfFileThumbnailExists(string blobName)
        {
            var blob = ThumbnailFile(blobName);

            return blob.Exists();
        }

        #endregion

        #region files
        public void DeleteFile(string fileName)
        {
            CloudBlockBlob blob = GetFile(fileName);

            //var options = new BlobRequestOptions
            //{

            //    DeleteSnapshotsOption = DeleteSnapshotsOption.IncludeSnapshots,
            //};


            blob.BeginDeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots,
                null,
                new BlobRequestOptions()
                                , new OperationContext(),
                async => blob.EndDeleteIfExists(async), null);

            //blob.BeginDeleteIfExists(options, async => blob.EndDeleteIfExists(async), null);

        }

        /// <summary>
        /// Download file from storage
        /// </summary>
        /// <param name="fileName">The blob name</param>
        /// <param name="aheadSize">Size to be read by the stream - if you want all the blob don't pass this parameter</param>
        /// <returns>Stream of the blob</returns>
        public Stream DownloadFile(string fileName, long? aheadSize)
        {
            CloudBlockBlob blob = GetFile(fileName);

            var sr = blob.OpenRead();
            //if (aheadSize.HasValue)
            //{
            //    sr.ReadAheadSize = aheadSize.Value; // ensure all blob will be 
            //}
            return sr;
        }



        public Stream DownloadFile(string fileName)
        {
            CloudBlockBlob blob = GetFile(fileName);

            using (var ms = new MemoryStream())
            {
                blob.DownloadToStream(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            CloudBlockBlob blob = GetFile(fileName);
            var ms = new MemoryStream();
            await blob.DownloadToStreamAsync(ms);
            //await Task.Factory.FromAsync<MemoryStream>(blob.BeginDownloadToStream, blob.EndDownloadToStream, ms, null);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        //public byte[] DownloadFileToBytes(string fileName)
        //{

        //    CloudBlockBlob blob = GetFile(fileName);
        //    using (var ms = new MemoryStream())
        //    {
        //        blob.DownloadToStream(ms);
        //        return ms.ToArray();
        //    }
        //    //return blob.DownloadByteArray();
        //}
        //public bool CheckIfFileExists(string blobName)
        //{
        //    var blob = GetFile(blobName);
        //    return blob.Exists();
        //}

        #endregion


        #region FAQRegion
        public async Task<Stream> GetFaqQeustion()
        {
            try
            {
                var blob = m_BlobClient.GetContainerReference(AzureFaQContainer).GetBlockBlobReference("help.xml");
                return await blob.OpenReadAsync();
            }
            catch (StorageException)
            {
                return null;
            }
        }
        #endregion





    }
}
