using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Core.Interfaces;
using Microsoft.WindowsAzure.Storage.Blob;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2.FileProcessor
{
    public class AudioProcessor : IFileProcessor
    {
        private readonly IVideoService _videoService;
        private readonly ICommandBus _commandBus;

        public AudioProcessor(IVideoService videoService, ICommandBus commandBus)
        {
            _videoService = videoService;
            _commandBus = commandBus;
        }

        public async Task ProcessFileAsync(long id, CloudBlockBlob blob, ILogger log, CancellationToken token)
        {
            var signedUrl = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow + TimeSpan.FromHours(6)

            });
            var url = new Uri(blob.Uri, signedUrl);
            await _videoService.CreateAudioPreviewJobAsync(id, url.AbsoluteUri, token);
        }
    }
}
