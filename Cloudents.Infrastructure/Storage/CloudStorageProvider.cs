using Cloudents.Core.Storage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.Infrastructure.Storage
{
    public interface ICloudStorageProvider
    {
        CloudBlobDirectory GetBlobClient(IStorageContainer container);
        StorageCredentials GetCredentials();
    }

    public class CloudStorageProvider : ICloudStorageProvider, Autofac.IStartable
    {
        private CloudStorageAccount CloudStorage { get; }

        public CloudStorageProvider(string connectionString)
        {
            //_connectionString = connectionString;
            CloudStorage = CloudStorageAccount.Parse(connectionString);
        }

        public CloudBlobDirectory GetBlobClient(IStorageContainer container)
        {
            var blobClient = CloudStorage.CreateCloudBlobClient();
            var con = blobClient.GetContainerReference(container.Name.ToLowerInvariant());
            return con.GetDirectoryReference(container.RelativePath ?? string.Empty);
        }

        public StorageCredentials GetCredentials()
        {
            return CloudStorage.Credentials;
        }

        public void Start()
        {
            //If we want to create new storage
        }
    }
}
