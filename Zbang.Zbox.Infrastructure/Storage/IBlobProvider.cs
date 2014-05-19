using System;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using Zbang.Zbox.Infrastructure.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IBlobProvider
    {
        string GenerateSharedAccressReadPermissionInCache(string blobName, double experationTimeInMinutes);
        //string GenerateSharedAccessWritePermissionBlobFiles(CloudBlockBlob blob, double experationTimeInMinutes);

        //string FetchBlobMimeType(string fileName);

        /// <summary>
        /// Upload file to cache container
        /// </summary>
        /// <param name="blobName">The blob Name</param>
        /// <param name="fileContent">the file content</param>
        /// <param name="mimeType">mimetype of the file</param>
        /// <returns>The url of the file with shared access signature</returns>
        Task<string> UploadFileToCacheAsync(string blobName, byte[] fileContent, string mimeType, bool fileGziped = false);
        Task<string> UploadFileToCacheAsync(string blobName, Stream fileContent, string mimeType, bool fileGziped = false);
       // bool CheckIfFileExistsInCache(string blobName);
       // string GetFileUrlInCahce(string blobName);
        //void DeleteCahceContent();

        //TODO:maybe we want this to be internal
        //Uri UploadProfilePicture(string blobName, byte[] fileContent, ImageSize imageSize);
        Uri UploadProfilePicture(string blobName, byte[] fileContent);


        void UploadFileThumbnail(string fileName, Stream ms, string mimeType);
        Task UploadFileThumbnailAsync(string fileName, Stream ms, string mimeType);

        bool CheckIfFileThumbnailExists(string blobName);

        CloudBlockBlob GetFile(string blobName);
        void DeleteFile(string fileName);

        Stream DownloadFile(string fileName, long? aheadSize);
        Stream DownloadFile(string fileName);
        Task<Stream> DownloadFileAsync(string fileName);

        //byte[] DownloadFileToBytes(string fileName);
       // bool CheckIfFileExists(string blobName);

        Task<int> UploadFileBlockAsync(string blobName, Stream fileContent, int currentIndex);
        Task UploadFileAsync(string blobName, string filePath, string mimeType);
        Task CommitBlockListAsync(string blobName, int currentIndex, string contentType);

        //Task<IEnumerable<string>> UploadFileBlockAsync(string blobName, Stream fileContent);
        //Task CommitBlockListAsync(string blobName, IEnumerable<string> blockList, string contentType);
        Task<long> UploadFromLinkAsync(string url, string fileName);


        Task SaveMetaDataToBlobAsync(string blobName, IDictionary<string, string> metaData);
        Task<IDictionary<string, string>> FetechBlobMetaDataAsync(string blobName);

        string GenerateSharedAccressReadPermissionInStorage(Uri blobUri, double experationTimeInMinutes);


        Task<Stream> GetFAQQeustion();

        string GenerateSharedAccressReadPermissionInCacheWithoutMeta(string blobName, double experationTimeInMinutes);


        string ProfileContainerUrl { get; }

        string BlobContainerUrl { get; }

        string GetThumbnailUrl(string blobName);
        string GetThumbnailLinkUrl();
        string GetBlobUrl(string blobName);
    }
}
