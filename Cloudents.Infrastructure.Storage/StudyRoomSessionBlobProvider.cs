//using System;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.Storage;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Blob;

//namespace Cloudents.Infrastructure.Storage
//{
//    public class StudyRoomSessionBlobProvider : BlobProviderContainer, IStudyRoomSessionBlobProvider
//    {
//        public StudyRoomSessionBlobProvider(ICloudStorageProvider storageProvider) : base(storageProvider)
//        {
//        }

//        public StudyRoomSessionBlobProvider(ICloudStorageProvider storageProvider, StorageContainer container) : base(storageProvider, container)
//        {
//        }

//        public Uri DownloadVideoLink(Guid roomId, string sessionId)
//        {
//            var blobName = BuildBlobName(roomId, sessionId);
//            var blob = GetBlob(blobName);
//            return blob.Uri;
//        }

//        public Task UploadVideoAsync(Guid roomId, string sessionId, Stream stream, CancellationToken token)
//        {
//            var blobName = BuildBlobName(roomId, sessionId);

//            var blob = GetBlob(blobName);
//            if (stream.CanSeek)
//            {
//                stream.Seek(0, SeekOrigin.Begin);
//            }
//            blob.Properties.ContentType = "video/webm";
//            return blob.UploadFromStreamAsync(stream, AccessCondition.GenerateIfNoneMatchCondition("*"), new BlobRequestOptions(), new OperationContext(), token);
//        }

//        private static string  BuildBlobName(Guid roomId, string sessionId)
//        {
//            return $"{roomId}/{sessionId}.mp4";
//        }
//    }
//}