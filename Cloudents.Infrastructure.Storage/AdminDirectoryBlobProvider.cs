using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Storage
{
    public class AdminDirectoryBlobProvider : BlobProviderContainer, IAdminDirectoryBlobProvider
    {
        public AdminDirectoryBlobProvider(ICloudStorageProvider storageProvider)
            : base(storageProvider, StorageContainer.Admin)
        {
        }
    }
}
