using Cloudents.Core.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Storage
{
    public class UserDirectoryBlobProvider : BlobProviderContainer, IUserDirectoryBlobProvider
    {

        private static readonly Dictionary<string, string> MimeTypeMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/gif"] = ".gif",
            ["image/jpeg"] = ".jpg",
            ["image/bmp"] = ".bmp",
        };

        public UserDirectoryBlobProvider(ICloudStorageProvider storageProvider)
            : base(storageProvider, StorageContainer.User)
        {
        }

        public async Task<Uri> UploadImageAsync(long userId, string file,
                         Stream stream, string contentType, CancellationToken token)
        {
           
            if (!MimeTypeMapping.ContainsKey(contentType))
            {
                throw new ArgumentException();
            }
            var extension = Path.GetExtension(file);
            if (string.IsNullOrEmpty(extension))
            {
                extension = MimeTypeMapping[contentType];
            }
            var fileName = $"{userId}/{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{extension}";
            var fileUri = GetBlobUrl(fileName);

            await UploadStreamAsync(fileName, stream,
                                  contentType, TimeSpan.FromDays(365), token: token);
            return fileUri;
        }
    }
}
