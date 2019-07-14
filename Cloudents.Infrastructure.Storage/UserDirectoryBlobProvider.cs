﻿using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Storage
{
    public class UserDirectoryBlobProvider: BlobProviderContainer, IUserDirectoryBlobProvider
    {
        private readonly IBinarySerializer _serializer;
        public UserDirectoryBlobProvider(ICloudStorageProvider storageProvider, IBinarySerializer serializer)
            : base(storageProvider, StorageContainer.User)
        {
            _serializer = serializer;
        }
     
        public async Task<byte[]> UploadImageAsync(long userId, string file,
                         Stream stream, string contentType, CancellationToken token)
        {
            var extension = Path.GetExtension(file);
            string[] supportedImages = { ".jpg", ".png", ".gif", ".jpeg", ".bmp" };
            if (!supportedImages.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return null;
            }

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