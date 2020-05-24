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
            string? contentDisposition = null);

        Task UploadStreamAsync(string blobName, Stream fileContent,
            string? mimeType = null, TimeSpan? cacheControlTime = null, CancellationToken token = default);

        Task UploadBlockFileAsync(string blobName, Stream fileContent, int index, CancellationToken token);
        Task CommitBlockListAsync(string blobName, string mimeType, string? originalFileName, IEnumerable<int> indexes, TimeSpan? cacheControlTime = null, CancellationToken token = default);


        Uri GetBlobUrl(string blobName, bool cdn = false);

        Task MoveAsync(string blobName, string destinationContainerName, CancellationToken token);

       // Task<IEnumerable<Uri>> FilesInDirectoryAsync(string directory, CancellationToken token);
        Task<IEnumerable<Uri>> FilesInContainerAsync(CancellationToken token);
        Task<IEnumerable<Uri>> FilesInDirectoryAsync(string prefix, string directory, CancellationToken token);

       

        Task DeleteDirectoryAsync(string id, CancellationToken token);
        Task UnDeleteDirectoryAsync(string id, CancellationToken token);
    }

    public interface IDocumentDirectoryBlobProvider : IBlobProvider
    {
        Uri GetPreviewImageLink(long id, int i);
        Task<string?> DownloadTextAsync(string name, string directory, CancellationToken token);
    }

    public interface IUserDirectoryBlobProvider : IBlobProvider
    {
        Task<Uri> UploadImageAsync(long userId, string file,
            Stream stream, string contentType, CancellationToken token = default);
    }

    public interface IAdminDirectoryBlobProvider : IBlobProvider
    {
    }

    public interface IChatDirectoryBlobProvider : IBlobProvider
    {

    }

    //public interface IStudyRoomSessionBlobProvider : IBlobProvider
    //{
    //    Task UploadVideoAsync(Guid roomId, string sessionId, Stream stream, CancellationToken token);
    //    Uri DownloadVideoLink(Guid roomId, string sessionId);

    //}


}
