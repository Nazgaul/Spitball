using System;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Storage
{
    public class FilesBlobProvider : BlobProviderContainer, IDocumentDirectoryBlobProvider
    {
       

        public FilesBlobProvider(ICloudStorageProvider storageProvider) : base(storageProvider, StorageContainer.File)
        {
        }

        public Uri GetPreviewImageLink(long id, int i)
        {
            var destinationDirectory = _blobDirectory.GetDirectoryReference(id.ToString());
            var blob = destinationDirectory.GetBlobReference($"preview-{i}.jpg");
            return blob.Uri;
        }

    }
}