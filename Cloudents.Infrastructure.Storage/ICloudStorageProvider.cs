using Azure.Storage;
using Azure.Storage.Blobs;


namespace Cloudents.Infrastructure.Storage
{
    public interface ICloudStorageProvider
    {
        BlobServiceClient GetBlobClient();

        //string AccountKey();
        // CloudQueueClient GetQueueClient();
    }


}