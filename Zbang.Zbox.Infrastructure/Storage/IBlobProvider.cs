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
        Task<Stream> OpenBlobStreamAsync(Uri blobUri, CancellationToken cancelToken);
         Task<IDictionary<string, string>> FetchBlobMetaDataAsync(Uri blobUri, CancellationToken token);
        Task SaveMetaDataToBlobAsync(Uri blobUri, IDictionary<string, string> metadata, CancellationToken token);

        string GenerateSharedAccessReadPermissionInStorage(Uri blobUri, double expirationTimeInMinutes);


        string StorageContainerUrl { get; }
    }
}