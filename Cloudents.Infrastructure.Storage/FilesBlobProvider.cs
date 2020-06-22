using Cloudents.Core.Storage;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Storage
{
    public class FilesBlobProvider : BlobProviderContainer, IDocumentDirectoryBlobProvider
    {


        public FilesBlobProvider(IConfigurationKeys storageProvider) : base(storageProvider, StorageContainer.File)
        {
        }

        public Uri GetPreviewImageLink(long id, int i)
        {
            var blob = GetBlob($"{id}/preview-{i}.jpg");
            return blob.Uri;
        }

        public async Task<string?> DownloadTextAsync(string name, string directory, CancellationToken token)
        {
            try
            {
                var blob = GetBlob($"{directory}/name");
                var x = await blob.DownloadAsync(token);
                using var streamReader = new StreamReader(x.Value.Content);
                return await streamReader.ReadToEndAsync();
            }
            catch (RequestFailedException e)
            {
                if ( e.Status == (int)HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }

    }
}