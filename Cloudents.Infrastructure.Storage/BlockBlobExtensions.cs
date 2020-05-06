using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.Infrastructure.Storage
{
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

        public static async Task RenameBlobAsync(this CloudBlockBlob blob, string newName)
        {
            var newBlobName = blob.Container.GetBlockBlobReference(newName);
            //var dir = blob.Parent;
           // var newBlobName = dir.GetBlockBlobReference(newName);

            await newBlobName.StartCopyAsync(blob);
            await blob.DeleteAsync();
        }
    }
}