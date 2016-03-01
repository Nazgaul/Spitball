using Microsoft.WindowsAzure.Storage.Blob;

namespace Zbang.Zbox.Infrastructure.Azure.Blob
{
    public interface ICloudBlockProvider
    {
        CloudBlockBlob GetFile(string blobName);
    }

    public interface IBlobUpload
    {
        string GenerateWriteAccessPermissionToBlob(string blobName, string mimeType);
        string GenerateReadAccessPermissionToBlob(string blobName);
    }

    


}
