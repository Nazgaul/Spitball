using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Storage
{
    public class AdminDirectoryBlobProvider : BlobProviderContainer, IAdminDirectoryBlobProvider
    {
        public AdminDirectoryBlobProvider(IConfigurationKeys storageProvider)
            : base(storageProvider, StorageContainer.Admin)
        {
         
        }
        public async IAsyncEnumerable<Uri> FilesInContainerAsync(CancellationToken token)
        {
            await foreach (var page in _cloudContainer.GetBlobsAsync().AsPages().WithCancellation(token))
            {
                foreach (var pageValue in page.Values)
                {
                    var blob = GetBlob(pageValue.Name);
                    yield return blob.Uri;
                }
            }

        }
    }
}
