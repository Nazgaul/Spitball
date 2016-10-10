using System;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IBlobProvider
    {
        //TODO:maybe we want this to be internal
        Task<Uri> UploadProfilePictureAsync(string blobName, Stream fileContent);
        Task<Stream> DownloadFileAsync(Uri blobUrl, CancellationToken cancelToken);
        Task<string> DownloadToLocalDiskAsync(Uri blobUrl, CancellationToken cancelToken);

       
        Task<IDictionary<string, string>> FetchBlobmetaDataAsync(Uri blobUri, CancellationToken token);
        Task SaveMetaDataToBlobAsync(Uri blobUri, IDictionary<string, string> metadata, CancellationToken token);

        string GenerateSharedAccessReadPermissionInStorage(Uri blobUri, double expirationTimeInMinutes);

        Task<Stream> GetFaqQuestionAsync();
        Task<Stream> GetJobsXmlAsync();

        string StorageContainerUrl { get; }

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