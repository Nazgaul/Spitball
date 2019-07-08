using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Storage
{
    public class UserDirectoryBlobProvider: BlobProviderContainer, IUserDirectoryBlobProvider
    {
        private readonly IBinarySerializer _serializer;
        public UserDirectoryBlobProvider(ICloudStorageProvider storageProvider, StorageContainer container, IBinarySerializer serializer)
            : base(storageProvider, container)
        {
            _serializer = serializer;
        }
     
        public async Task<byte[]> GetImageUrl(long userId, string extension,
                         Stream stream, string contentType, CancellationToken token)
        {
            var fileName = $"{userId}/{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{extension}";
            var fileUri = GetBlobUrl(fileName);
            var imageProperties = new ImageProperties(fileUri, ImageProperties.BlurEffect.None);

            var hash = _serializer.Serialize(imageProperties);
            await UploadStreamAsync(fileName, stream,
                                  contentType, TimeSpan.FromDays(365), token: token);
            return hash;
        }
    }
}
