using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IBlobProvider2<out T> where T : IStorageContainerName //: IBlobProvider
    {

        Task<int> UploadFileBlockAsync(string blobName, Stream fileContent, int currentIndex);
        Task CommitBlockListAsync(string blobName, int currentIndex, string contentType);
        Task UploadStreamAsync(string blobName, Stream content, string mimeType, CancellationToken token);

        Task RemoveBlobAsync(string blobName, CancellationToken token);
        Uri GetBlobUrl(string blobName);
        Uri GetBlobUrl(string blobName , bool useCdn);

        Task<bool> ExistsAsync(string blobName);
        bool Exists(string blobName);
        Task<long> SizeAsync(string blobName);
        Task<string> Md5Async(string blobName);

        Task SaveMetaDataToBlobAsync(Uri blobUri, IDictionary<string, string> metadata, CancellationToken token);
        string RelativePath();

        Task UploadByteArrayAsync(string blobName, byte[] fileContent, string mimeType, bool fileGziped, int cacheControlMinutes);
        string GenerateSharedAccessReadPermission(string blobName, double expirationTimeInMinutes);
        string GenerateSharedAccessWritePermission(string blobName, string mimeType);

        string GenerateSharedAccessReadPermission(string blobName, double expirationTimeInMinutes,
            string contentDisposition);

        Task UploadFromLinkAsync(string url, string fileName);
    }
}