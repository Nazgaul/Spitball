using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.Infrastructure.Storage
{
    public static class BlockBlobExtensions
    {
        public static Uri GetDownloadLink([NotNull] this CloudBlockBlob blob, TimeSpan expirationTime)
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