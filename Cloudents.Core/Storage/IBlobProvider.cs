using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Storage
{

    public interface IBlobProvider
    {
        Uri GeneratePreviewLink(Uri blobUrl, double expirationTimeInMinutes);
        Uri GenerateDownloadLink(Uri blobUrl, double expirationTimeInMinutes,
            string contentDisposition);
    }

    public interface IBlobProvider<[UsedImplicitly] T> where T : IStorageContainer
    {
        Task UploadStreamAsync(string blobName, Stream fileContent,
            string mimeType = null, bool fileGziped = false, int? cacheControlSeconds = null, CancellationToken token = default);

        Task UploadBlockFileAsync(string blobName, Stream fileContent, int index, CancellationToken token);
        Task CommitBlockListAsync(string blobName, string mimeType, IList<int> indexes, CancellationToken token);


        //string GenerateDownloadLink(string blobName, double expirationTimeInMinutes,
        //    string contentDisposition);

        Uri GetBlobUrl(string blobName, bool cdn = false);

        Task MoveAsync(string blobName, string destinationContainerName, CancellationToken token);

        Task<IEnumerable<Uri>> FilesInDirectoryAsync(string directory, CancellationToken token);
        Task<IEnumerable<Uri>> FilesInDirectoryAsync(string prefix, string directory, CancellationToken token);


        Task<string> DownloadTextAsync(string name, string directory, CancellationToken token);
        /// <summary>
        /// Used to check if a blob exists - used in ico site
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="token"></param>
        /// <returns>true if blob exists other wise false</returns>
        Task<bool> ExistsAsync(string blobName, CancellationToken token);

        Task DeleteDirectoryAsync(string id, CancellationToken token);
    }
}
