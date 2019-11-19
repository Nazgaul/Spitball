using Cloudents.Core.Storage;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Storage
{
    public class UserDirectoryBlobProvider : BlobProviderContainer, IUserDirectoryBlobProvider
    {
        public UserDirectoryBlobProvider(ICloudStorageProvider storageProvider)
            : base(storageProvider, StorageContainer.User)
        {
        }

        public async Task<Uri> UploadImageAsync(long userId, string file,
                         Stream stream, string contentType, CancellationToken token)
        {
            var extension = Path.GetExtension(file);
            string[] supportedImages = { "image/jpg", "image/png", "image/gif", "image/jpeg", "image/bmp" };
            if (!supportedImages.Contains(contentType, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException();
            }
            var fileName = $"{userId}/{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{extension}";
            var fileUri = GetBlobUrl(fileName);

            await UploadStreamAsync(fileName, stream,
                                  contentType, TimeSpan.FromDays(365), token: token);
            return fileUri;
        }
    }
}
