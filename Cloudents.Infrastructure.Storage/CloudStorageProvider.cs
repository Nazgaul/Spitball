using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Cloudents.Infrastructure.Storage
{
    public class CloudStorageProvider : ICloudStorageProvider, Autofac.IStartable
    {
        private CloudStorageAccount CloudStorage { get; }

        public CloudStorageProvider(string connectionString)
        {
            CloudStorage = CloudStorageAccount.Parse(connectionString);
        }

        public CloudBlobClient GetBlobClient(/*IStorageContainer container*/)
        {
            var blobClient = GetCloudBlobClient();
            return blobClient;
            //var att = ExtractContainerData(container);
            // var con = blobClient.GetContainerReference(container.Container.Name.ToLowerInvariant());

            //return con.GetDirectoryReference(container.Container.RelativePath ?? string.Empty);
        }

        //private static StorageAttribute ExtractContainerData(StorageContainer container)
        //{
        //    return container.GetType().GetField(container.ToString()).GetCustomAttribute<StorageAttribute>();
        //}

        private CloudBlobClient GetCloudBlobClient()
        {
            var blobClient = CloudStorage.CreateCloudBlobClient();
            //var att = container.GetType().GetField(container.ToString()).GetCustomAttribute<StorageAttribute>();
            //var con = blobClient.GetContainerReference(att.Name.ToLowerInvariant());
            return blobClient;
        }

        public CloudQueueClient GetQueueClient()
        {
            return CloudStorage.CreateCloudQueueClient();
        }

        public StorageCredentials GetCredentials()
        {
            return CloudStorage.Credentials;
        }

        public void Start()
        {
            //var client = GetQueueClient();

            //var tasks = new List<Task>();
            //foreach (var queueName in GetQueues())
            //{
            //    var queue = client.GetQueueReference(queueName.Name);
            //    tasks.Add(queue.CreateIfNotExistsAsync());
            //}

            //var storageClient = GetCloudBlobClient();
            //foreach (var container in GetContainers())
            //{
            //    //var att = ExtractContainerData(container);
            //    var blobContainer = storageClient.GetContainerReference(container.Name);
            //    tasks.Add(blobContainer.CreateIfNotExistsAsync());
            //}


            //Task.WaitAll(tasks.ToArray(), TimeSpan.Zero);

            //If we want to create new storage
        }

        //private static IEnumerable<QueueName> GetQueues()
        //{
        //    foreach (var field in typeof(QueueName).GetFields(BindingFlags.Public | BindingFlags.Static))
        //    {
        //        if (field.IsLiteral)
        //        {
        //            continue;
        //        }
        //        yield return (QueueName)field.GetValue(null);
        //    }
        //}


        //public static IEnumerable<StorageContainer> GetContainers()
        //{
        //    // return Enum.GetValues(typeof(StorageContainer)).Cast<StorageContainer>();
        //    foreach (var field in typeof(StorageContainer).GetFields(BindingFlags.Public | BindingFlags.Static))
        //    {
        //        if (field.IsLiteral)
        //        {
        //            continue;
        //        }
        //        yield return (StorageContainer)field.GetValue(null);
        //    }
        //}
    }
}
