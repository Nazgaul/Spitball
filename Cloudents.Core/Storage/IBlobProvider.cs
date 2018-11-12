using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Cloudents.Core.Storage
{
    public interface IBlobProvider<[UsedImplicitly] T> where T : IStorageContainer
    {
        Task UploadStreamAsync(string blobName, Stream fileContent,
            string mimeType = null, bool fileGziped = false, int? cacheControlSeconds = null, CancellationToken token = default);

        Task UploadBlockFileAsync(string blobName, Stream fileContent, int index, CancellationToken token);
       // Task CommitBlockListAsync(string blobName, IList<int> indexes, CancellationToken token);
        Task CommitBlockListAsync(string blobName, string mimeType, IList<int> indexes, CancellationToken token);

        //string GenerateSharedAccessReadPermission(string blobName, double expirationTimeInMinutes);

        string GenerateDownloadLink(string blobName, double expirationTimeInMinutes,
            string contentDisposition);

       // Task<bool> ExistsAsync(string blobName, CancellationToken token);
        Uri GetBlobUrl(string blobName, bool cdn = false);

        Task MoveAsync(string blobName, string destinationContainerName, CancellationToken token);

        Task<IEnumerable<Uri>> FilesInDirectoryAsync(string directory, CancellationToken token);
        Task<IEnumerable<Uri>> FilesInDirectoryAsync(string prefix, string directory, CancellationToken token);

        //Task<Stream> DownloadFileAsync(string blobUrl, CancellationToken token);
        //Task<IDictionary<string, string>> FetchBlobMetaDataAsync(string blobUri, CancellationToken token);
        //Task SaveMetaDataToBlobAsync(string blobUri, IDictionary<string, string> metadata, CancellationToken token);
    }

    
}
