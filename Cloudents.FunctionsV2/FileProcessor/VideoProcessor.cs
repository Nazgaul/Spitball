using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Infrastructure.Video;
using JetBrains.Annotations;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2.FileProcessor
{
    public class VideoProcessor : IFileProcessor
    {
        private readonly IVideoService _videoService;
        private readonly ICommandBus _commandBus;


        public VideoProcessor(IVideoService videoService, ICommandBus commandBus)
        {
            _videoService = videoService;
            _commandBus = commandBus;
        }

        public async Task ProcessFileAsync(long id, CloudBlockBlob blob, IBinder binder, ILogger log, CancellationToken token)
        {
            var signedUrl = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow + TimeSpan.FromHours(6)

            });
            var url = new Uri(blob.Uri, signedUrl);
            await _videoService.CreateVideoPreviewJobAsync(id, url.AbsoluteUri, token);
        }

        public async Task MoveStudyRoomVideoAsync(string assetName, IBinder binder, CancellationToken token)
        {
            var name = assetName.Replace(AssetType.StudyRoom.ToString(), string.Empty).Trim('-').Split(StudyRoomFunctionBlobScan.Separator);
            var studyRoomId = name[0];
            var sessionId = name[1];



            //binder.BindAsync<CloudBlob>(new BlobAttribute(""))
            var assetContainer = await binder.BindAsync<CloudBlobContainer>(new BlobAttribute(assetName), token);
            var blobs = await assetContainer.ListBlobsSegmentedAsync(null);
            var blobItem = blobs.Results.SingleOrDefault(w => w.Uri.AbsoluteUri.EndsWith("mp4"));
            if (!(blobItem is null))
            {
                var blob = (CloudBlockBlob)blobItem;
                var url = blob.GetDownloadLink(TimeSpan.FromHours(1));


                var destinationBlob = await binder.BindAsync<CloudBlockBlob>(
                    new BlobAttribute(
                        $"{StorageContainer.StudyRoom.Name}/{StorageContainer.StudyRoom.RelativePath}/{studyRoomId}/{studyRoomId}_{sessionId}.mp4"),
                    token);

                destinationBlob.Metadata[StudyRoomFunctionBlobScan.MetaEncodingKey] = "Finish";

                await destinationBlob.StartCopyAsync(url);

                while (destinationBlob.CopyState.Status != CopyStatus.Success)
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.2), token);
                    await destinationBlob.ExistsAsync();
                }
            }

            await _videoService.DeleteAssetAsync(assetName, token);
        }

        public async Task MoveImageAsync(long id, IBinder binder, CancellationToken token)
        {
            var containerName = await _videoService.GetAssetContainerAsync(id, AssetType.Thumbnail, token);
            if (containerName == null)
            {
                return;
            }
            var container = await binder.BindAsync<CloudBlobContainer>(new BlobAttribute(containerName), token);
            if (await container.ExistsAsync())
            {
                var blobs = await container.ListBlobsSegmentedAsync(MediaServices.PrefixThumbnailBlobName, null);
                var blob = (CloudBlockBlob)blobs.Results.SingleOrDefault();
                if (!(blob is null))
                {
                    var url = blob.GetDownloadLink(TimeSpan.FromHours(1));

                    var destinationBlob = await binder.BindAsync<CloudBlockBlob>(
                        new BlobAttribute(
                            $"{StorageContainer.File.Name}/{StorageContainer.File.RelativePath}/{id}/preview-0.jpg"),
                        token);


                    await destinationBlob.StartCopyAsync(url);

                    while (destinationBlob.CopyState.Status != CopyStatus.Success)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(0.2), token);
                        await destinationBlob.ExistsAsync();
                    }
                }

                await _videoService.DeleteImageAssetAsync(id, token);
            }

        }

        public Task CreateLocatorAsync(long id, CancellationToken token)
        {
            return _videoService.CreateShortStreamingLocator(id, token);
        }

       

        public async Task UpdateDurationAsync(long id, IBinder binder, CancellationToken token)
        {
            //var id = RegEx.NumberExtractor.Match(assetName).Value;
            var containerName = await _videoService.GetAssetContainerAsync(id, AssetType.Long, token);
            var container = await binder.BindAsync<CloudBlobContainer>(new BlobAttribute(containerName), token);
            var blobs = await container.ListBlobsSegmentedAsync(null, null);
            var blob = blobs.Results.OfType<CloudBlockBlob>()
                .First(f => f.Name.EndsWith("manifest.json"));

            var str = await blob.DownloadTextAsync();
            dynamic json = JToken.Parse(str);
            string durationXml = json.AssetFile[0].Duration;

            var v = XmlConvert.ToTimeSpan(durationXml);


            var blobDirectoryFiles = await binder.BindAsync<IEnumerable<CloudBlockBlob>>(new BlobAttribute($"spitball-files/files/{id}"), token);
            var originalVideoBlob =
                blobDirectoryFiles.Single(s => s.Name.Contains("/file-", StringComparison.OrdinalIgnoreCase));
            var url = originalVideoBlob.GetDownloadLink(TimeSpan.FromHours(1));
            await _videoService.CreatePreviewJobAsync(id, url.AbsoluteUri, v, token);

            var command = UpdateDocumentMetaCommand.Video(id, v);
            await _commandBus.DispatchAsync(command, token);
        }

    }

    public static class BlockBlobExtensions
    {
        public static Uri GetDownloadLink(this CloudBlockBlob blob, TimeSpan expirationTime)
        {
            if (blob == null) throw new ArgumentNullException(nameof(blob));
            var signedUrl = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow + expirationTime

            });
            var url = new Uri(blob.Uri, signedUrl);
            return url;
        }
    }



}
