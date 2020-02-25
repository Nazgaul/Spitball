using Cloudents.Core.Storage;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Storage
{
    public class AdminDirectoryBlobProvider : BlobProviderContainer, IAdminDirectoryBlobProvider
    {
        public AdminDirectoryBlobProvider(ICloudStorageProvider storageProvider)
            : base(storageProvider, StorageContainer.Admin)
        {
        }

        public async Task<Uri> UploadImageAsync(string file,
                         Stream stream, string contentType, CancellationToken token)
        {
            var extension = Path.GetExtension(file);
            var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{extension}";
            var fileUri = GetBlobUrl(fileName);

            await UploadStreamAsync(fileName, stream,
                                  contentType, TimeSpan.FromDays(365), token: token);
            return fileUri;
        }
    }
}
