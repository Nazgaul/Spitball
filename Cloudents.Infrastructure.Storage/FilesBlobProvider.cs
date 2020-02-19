using Cloudents.Core.Storage;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task<string> DownloadTextAsync(string name, string directory, CancellationToken token)
        {
            try
            {
                var destinationDirectory = _blobDirectory.GetDirectoryReference(directory);
                var blob = destinationDirectory.GetBlockBlobReference(name);

                return await blob.DownloadTextAsync();
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }

    }
}