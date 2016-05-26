using System;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IBlobProvider2<out T> where T : IStorageContainerName //: IBlobProvider
    {

        Task<int> UploadFileBlockAsync(string blobName, Stream fileContent, int currentIndex);
        Task CommitBlockListAsync(string blobName, int currentIndex, string contentType);
        Task UploadStreamAsync(string blobName, Stream content, string mimeType, CancellationToken token);
        string GetBlobUrl(string blobName);

        Task<bool> ExistsAsync(string blobName);
        bool Exists(string blobName);
    }

    public interface IBlobProvider
    {
        string GenerateSharedAccressReadPermissionInCache(string blobName, double expirationTimeInMinutes);

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
        Task<Uri> UploadProfilePictureAsync(string blobName, Stream fileContent);

        Task<bool> ExistsAsync(Uri blobUri);
        //Task UploadFilePreviewAsync(string blobName, Stream content, string mimeType, CancellationToken token = default(CancellationToken));
        // Task UploadFilePreviewAsync(Uri blobUri, Stream content, string mimeType, CancellationToken token);


        // Stream DownloadFile(string fileName);

        //Task<Stream> DownloadFileAsync(string fileName, CancellationToken cancelToken);
        Task<Stream> DownloadFileAsync(Uri blobUrl, CancellationToken cancelToken);
        Task<string> DownloadToLocalDiskAsync(Uri blobUrl, CancellationToken cancelToken);


        //Task<int> UploadFileBlockAsync(string blobName, Stream fileContent, int currentIndex);

        //Task<int> UploadFileBlockAsync(string blobName, string container, Stream fileContent,
        //int currentIndex);
        //Task CommitBlockListAsync(string blobName, int currentIndex, string contentType);
        //Task CommitBlockListAsync(string blobName, string container, int currentIndex, string contentType);

        Task<long> UploadFromLinkAsync(string url, string fileName);


        //Task SaveMetaDataToBlobAsync(string blobName, IDictionary<string, string> metaData, CancellationToken token);
        //Task<IDictionary<string, string>> FetchBlobMetaDataAsync(string blobName);
        Task<IDictionary<string, string>> FetchBlobMetaDataAsync(Uri blobUri, CancellationToken token);
        Task SaveMetaDataToBlobAsync(Uri blobUri, IDictionary<string, string> metaData, CancellationToken token);

        string GenerateSharedAccessReadPermissionInStorage(Uri blobUri, double expirationTimeInMinutes);

        Task<Stream> GetFaqQuestionAsync();
        Task<Stream> GetJobsXmlAsync();

        string GenerateSharedAccressReadPermissionInCacheWithoutMeta(string blobName, double expirationTimeInMinutes);


        //string ProfileContainerUrl { get; }

        //string BlobContainerUrl { get; }
        string StorageContainerUrl { get; }

        //string GetBlobUrl(string blobName);


        /// <summary>
        /// Upload image to quiz in create quiz page
        /// </summary>
        /// <param name="content">the image itself</param>
        /// <param name="mimeType">mime type of the image</param>
        /// <param name="boxId">box id</param>
        /// <param name="fileName">the image name received from client</param>
        /// <returns>url to return to client</returns>
        Task<string> UploadQuizImageAsync(Stream content, string mimeType, long boxId, string fileName);





    }
}