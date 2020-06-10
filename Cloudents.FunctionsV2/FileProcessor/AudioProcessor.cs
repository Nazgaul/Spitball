﻿using Cloudents.Core.Interfaces;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2.FileProcessor
{
    public class AudioProcessor : IFileProcessor
    {
        private readonly IVideoService _videoService;

        public AudioProcessor(IVideoService videoService)
        {
            _videoService = videoService;
        }

        public Task ProcessFileAsync(long id, CloudBlockBlob blob,IBinder binder, ILogger log, CancellationToken token)
        {
            var signedUrl = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow + TimeSpan.FromHours(6)

            });
            var url = new Uri(blob.Uri, signedUrl);
            return _videoService.CreateAudioPreviewJobAsync(id, url.AbsoluteUri, token);
        }
    }
}
