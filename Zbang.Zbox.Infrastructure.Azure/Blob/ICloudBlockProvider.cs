using Microsoft.WindowsAzure.Storage.Blob;

namespace Zbang.Zbox.Infrastructure.Azure.Blob
{
    public interface ICloudBlockProvider
    {
        CloudBlockBlob GetFile(string blobName);
        CloudBlockBlob GetFile(string blobName, string containerName);
    }
}
