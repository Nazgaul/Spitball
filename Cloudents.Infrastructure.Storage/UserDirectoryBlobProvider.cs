using Cloudents.Core.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Storage
{
    public class UserDirectoryBlobProvider : BlobProviderContainer, IUserDirectoryBlobProvider
    {
        private readonly IImageProcessor _imageProcessor;


        public static readonly Dictionary<string, string> MimeTypeMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/gif"] = ".gif",
            ["image/jpeg"] = ".jpg",
            ["image/bmp"] = ".bmp",
        };

        public UserDirectoryBlobProvider(IConfigurationKeys storageProvider, IImageProcessor imageProcessor)
            : base(storageProvider, StorageContainer.User)
        {
            _imageProcessor = imageProcessor;
        }

        public async Task<Uri> UploadImageAsync(long userId, string file,
                         Stream stream, string contentType, CancellationToken token)
        {

            if (!MimeTypeMapping.ContainsKey(contentType))
            {
                throw new ArgumentException($"content type not supported {contentType}");
            }

            var extension = Path.GetExtension(file);
            if (string.IsNullOrEmpty(extension))
            {
                extension = MimeTypeMapping[contentType];
            }

            var fileName = $"{userId}/{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{extension}";
            var fileUri = GetBlobUrl(fileName);
            await using var sr = _imageProcessor.ConvertToJpg(stream, maxWidth: 1920);
            await UploadStreamAsync(fileName, sr,
                                  contentType, TimeSpan.FromDays(365), token: token);
            return fileUri;
        }
    }


    public class StudyRoomBlobProvider : BlobProviderContainer, IStudyRoomBlobProvider
    {
        private readonly IImageProcessor _imageProcessor;


        public StudyRoomBlobProvider(IConfigurationKeys storageProvider, IImageProcessor imageProcessor)
            : base(storageProvider, StorageContainer.StudyRoom)
        {
            _imageProcessor = imageProcessor;
        }

        public async Task<Uri> UploadImageAsync(string file,
            Stream stream, string contentType, CancellationToken token)
        {
            if (!UserDirectoryBlobProvider.MimeTypeMapping.ContainsKey(contentType))
            {
                throw new ArgumentException($"content type not supported {contentType}");
            }

            await using var jpgImageStream = _imageProcessor.ConvertToJpg(stream);
            var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.jpg";
            var fileUri = GetBlobUrl(fileName);

            await UploadStreamAsync(fileName, jpgImageStream,
                "image/jpg", TimeSpan.FromDays(365), token: token);
            return fileUri;
        }
    }
}
