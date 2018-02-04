using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IBlobProvider2<out T> where T : IStorageContainerName //: IBlobProvider
    {
        Task UploadStreamAsync(string blobName, Stream content, string mimeType, CancellationToken token);

        Uri GetBlobUrl(string blobName);

        Task<bool> ExistsAsync(string blobName, CancellationToken token);
        bool Exists(string blobName);
        // ReSharper disable once InconsistentNaming
        Task<string> MD5Async(string blobName);

        string RelativePath();

        Task UploadByteArrayAsync(string blobName, byte[] fileContent, string mimeType, bool fileGziped, int cacheControlMinutes);
        string GenerateSharedAccessReadPermission(string blobName, double expirationTimeInMinutes);

        Task<string> DownloadTextAsync(string blobName);
    }
}