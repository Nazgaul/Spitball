﻿using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Net;
using Cloudents.Application.Storage;

namespace Cloudents.Infrastructure.Storage
{
    public class BlobProvider : IBlobProvider
    {
        private readonly ICloudStorageProvider _storageProvider;

        public BlobProvider(ICloudStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        private CloudBlockBlob GetBlob(Uri blobUrl)
        {
            return new CloudBlockBlob(blobUrl, _storageProvider.GetCredentials());
        }


        public Uri GeneratePreviewLink(Uri blobUrl, double expirationTimeInMinutes)
        {
            var blob = GetBlob(blobUrl);
            var signedUrl = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(expirationTimeInMinutes)

            });
            var url = new Uri(blob.Uri, signedUrl);
            return url;
        }

        public Uri GenerateDownloadLink(Uri blobUrl, double expirationTimeInMinutes, string fileName)
        {
            var blob = GetBlob(blobUrl);
            var signedUrl = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(expirationTimeInMinutes)

            }, new SharedAccessBlobHeaders
            {
                ContentDisposition = "attachment; filename=\"" + WebUtility.UrlEncode(fileName ?? blob.Name) + "\""
            });


            var url = new Uri(blob.Uri, signedUrl);
            return url;
        }
    }
}
