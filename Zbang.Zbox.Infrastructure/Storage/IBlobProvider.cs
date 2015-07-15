﻿using System;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IBlobProvider
    {
        string GenerateSharedAccressReadPermissionInCache(string blobName, double expirationTimeInMinutes);
        //string GenerateSharedAccessWritePermissionBlobFiles(CloudBlockBlob blob, double expirationTimeInMinutes);

        //string FetchBlobMimeType(string fileName);

        /// <summary>
        /// Upload file to cache container
        /// </summary>
        /// <param name="blobName">The blob Name</param>
        /// <param name="fileContent">the file content</param>
        /// <param name="mimeType">mimetype of the file</param>
        /// <param name="fileGziped"></param>
        /// <returns>The url of the file with shared access signature</returns>
        Task<string> UploadFileToCacheAsync(string blobName, byte[] fileContent, string mimeType,
            bool fileGziped = false);

        Task<string> UploadFileToCacheAsync(string blobName, Stream fileContent, string mimeType,
            bool fileGziped = false);


        //TODO:maybe we want this to be internal
        Uri UploadProfilePicture(string blobName, byte[] fileContent);


        void UploadFileThumbnail(string fileName, Stream ms, string mimeType);
        Task UploadFileThumbnailAsync(string fileName, Stream ms, string mimeType);
        Task UploadFileThumbnailAsync(string fileName, Stream ms, string mimeType, CancellationToken token);
        Task UploadFilePreviewAsync(string blobName, Stream content, string mimeType, CancellationToken token  = default(CancellationToken));

        bool CheckIfFileThumbnailExists(string blobName);


        Stream DownloadFile(string fileName);

        Task<Stream> DownloadFileAsync(string fileName);
        Task<Stream> DownloadFileAsync(string fileName, CancellationToken cancelToken);
        Task<Stream> DownloadFileAsync2(string fileName, CancellationToken cancelToken);
        Task<string> DownloadToFileAsync(string fileName, CancellationToken cancelToken);
        //Task DownloadFileToSystemAsync(string fileName, string systemLocation);

        Task<int> UploadFileBlockAsync(string blobName, Stream fileContent, int currentIndex);
        Task CommitBlockListAsync(string blobName, int currentIndex, string contentType);

        Task<long> UploadFromLinkAsync(string url, string fileName);


        Task SaveMetaDataToBlobAsync(string blobName, IDictionary<string, string> metaData);
        Task<IDictionary<string, string>> FetechBlobMetaDataAsync(string blobName);

        string GenerateSharedAccressReadPermissionInStorage(Uri blobUri, double expirationTimeInMinutes);

        Task<Stream> GetFaqQuestion();
        Task<Stream> GetJobsXml();

        string GenerateSharedAccressReadPermissionInCacheWithoutMeta(string blobName, double expirationTimeInMinutes);


        string ProfileContainerUrl { get; }

        string BlobContainerUrl { get; }

        string GetThumbnailUrl(string blobName);
        string GetThumbnailLinkUrl();
        string GetBlobUrl(string blobName);

        void RenameBlob(string blobName, string newName, string newMimeType = null);

        /// <summary>
        /// Upload image to quiz in create quiz page
        /// </summary>
        /// <param name="content">the image itself</param>
        /// <param name="mimeType">mime type of the image</param>
        /// <param name="boxId">box id</param>
        /// <param name="fileName">the image name received from client</param>
        /// <returns>url to return to client</returns>
        Task<string> UploadQuizImage(Stream content, string mimeType, long boxId, string fileName);

        bool CacheBlobExists(string blobName);
    }
}