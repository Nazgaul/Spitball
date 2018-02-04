using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Cloudents.Core.Storage;
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

        public CloudBlobDirectory GetBlobClient(IStorageContainer container)
        {
            var blobClient = CloudStorage.CreateCloudBlobClient();
            var con = blobClient.GetContainerReference(container.Name.ToLowerInvariant());
            return con.GetDirectoryReference(container.RelativePath ?? string.Empty);
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
            var client = GetQueueClient();


            var tasks = new List<Task>();
            foreach (var queueName in GetQueues())
            {
                var queue = client.GetQueueReference(queueName.Key);
                tasks.Add(queue.CreateIfNotExistsAsync());
            }

            Task.WaitAll(tasks.ToArray(), TimeSpan.MaxValue);

            //If we want to create new storage
        }

        private static IEnumerable<QueueName> GetQueues()
        {
            foreach (var field in typeof(QueueName).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                yield return (QueueName)field.GetValue(null);
            }
        }
    }
}
