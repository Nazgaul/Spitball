using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Cloudents.Infrastructure.Storage
{
    public interface ICloudStorageProvider
    {
        CloudBlobClient GetBlobClient();
        CloudQueueClient GetQueueClient();
    }
}