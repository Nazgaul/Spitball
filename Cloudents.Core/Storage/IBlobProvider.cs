﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Storage
{
    public interface IBlobProvider<T> where T : IStorageContainer
    {
        Task UploadByteArrayAsync(string blobName, byte[] fileContent,
            string mimeType, bool fileGziped, int cacheControlMinutes, CancellationToken token);

        string GenerateSharedAccessReadPermission(string blobName, double expirationTimeInMinutes);

        Task<bool> ExistsAsync(string blobName, CancellationToken token);
    }

    public interface IBlobProvider
    {
        Task<Stream> DownloadFileAsync(Uri blobUrl, CancellationToken token);
        Task<IDictionary<string, string>> FetchBlobMetaDataAsync(Uri blobUri, CancellationToken token);
        string GetBlobNameFromUri(Uri uri);
        Task SaveMetaDataToBlobAsync(Uri blobUri, IDictionary<string, string> metadata, CancellationToken token);
    }
}
