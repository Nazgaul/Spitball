﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Storage
{

    public interface IBlobProvider
    {
        Uri GeneratePreviewLink(Uri blobUrl, TimeSpan expirationTime);
        Uri GenerateDownloadLink(Uri blobUrl, TimeSpan expirationTime,
            string contentDisposition = null);

        Task UploadStreamAsync(string blobName, Stream fileContent,
            string mimeType = null, TimeSpan? cacheControlTime = null, CancellationToken token = default);

        Task UploadBlockFileAsync(string blobName, Stream fileContent, int index, CancellationToken token);
        Task CommitBlockListAsync(string blobName, string mimeType, IList<int> indexes, CancellationToken token);
        Task CommitBlockListAsync(string blobName, string mimeType, string originalFileName, IList<int> indexes, CancellationToken token);




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

    public interface IDocumentDirectoryBlobProvider : IBlobProvider
    {

    }

    public interface IQuestionsDirectoryBlobProvider : IBlobProvider
    {

    }

    public interface IUserDirectoryBlobProvider : IBlobProvider
    {

    }

    public interface IRequestTutorDirectoryBlobProvider : IBlobProvider
    {

    }

    public interface IChatDirectoryBlobProvider : IBlobProvider
    {

    }

    
}
