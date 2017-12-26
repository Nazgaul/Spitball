using Microsoft.WindowsAzure.Storage;

namespace Cloudents.Infrastructure.Storage
{
    public class CloudStorageProvider : Autofac.IStartable
    {
        private readonly string _connectionString;
        public CloudStorageAccount CloudStorage { get; }

        public CloudStorageProvider(string connectionString)
        {
            _connectionString = connectionString;
            CloudStorage = CloudStorageAccount.Parse(_connectionString);
        }


        public void Start()
        {
            //If we want to create new storage
        }
    }
}
