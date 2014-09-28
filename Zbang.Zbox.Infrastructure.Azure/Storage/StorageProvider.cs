using System.Configuration;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;
using Microsoft.WindowsAzure.Storage.Blob;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Zbang.Zbox.Infrastructure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Zbang.Zbox.Infrastructure.Azure.Table;
using System;

namespace Zbang.Zbox.Infrastructure.Azure.Storage
{
    internal static class StorageProvider
    {
        private static CloudStorageAccount _cloudStorageAccount;
        private static LocalResource _localStorage;

        static StorageProvider()
        {
            
            ConfigureStorageAccount();
            ConfigureLocalStorage();
        }
        private static void ConfigureLocalStorage()
        {
            if (RoleEnvironment.IsAvailable)
            {
                var azureLocalResource = RoleEnvironment.GetLocalResource("ItemPreviewStorage");
                _localStorage = new LocalResource { LocalResourcePath = azureLocalResource.RootPath, LocalResourceSizeInMegaBytes = azureLocalResource.MaximumSizeInMegabytes };
            }
            else
            {
                _localStorage = new LocalResource { LocalResourcePath = "c:\\Temp\\Zbox", LocalResourceSizeInMegaBytes = 200 };
                Directory.CreateDirectory(LocalResource.LocalResourcePath);
            }
        }
        private static void ConfigureStorageAccount()
        {
            try
            {
                _cloudStorageAccount = CloudStorageAccount.Parse(ConfigFetcher.Fetch("StorageConnectionString"));
            }
            catch (ArgumentNullException ex)
            {
                TraceLog.WriteError("on ConfigureStorageAccount", ex);
                _cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            }
            
            // not need every time
           // CreateBlobStorages(_cloudStorageAccount.CreateCloudBlobClient());
            //CreateQueues(_cloudStorageAccount.CreateCloudQueueClient());
            //CreateTables(_cloudStorageAccount.CreateCloudTableClient());

        }
        internal static LocalResource LocalResource
        {
            get
            {
                return _localStorage;
            }
        }
        internal static CloudStorageAccount ZboxCloudStorage
        {
            get
            {
                return _cloudStorageAccount;
            }
        }

        #region CreateStorage
        private static void CreateBlobStorages(CloudBlobClient blobClient)
        {
            var container = blobClient.GetContainerReference(BlobProvider.AzureBlobContainer.ToLower());

            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Off
                });
            }
            container = blobClient.GetContainerReference(BlobProvider.AzureCacheContainer.ToLower());
            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Off
                });
            }

            container = blobClient.GetContainerReference(BlobProvider.AzureProfilePicContainer.ToLower());
            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
            container = blobClient.GetContainerReference(BlobProvider.AzureThumbnailContainer.ToLower());
            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
            container = blobClient.GetContainerReference(BlobProvider.AzureFaQContainer.ToLower());
            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Off
                });
            }

            container = blobClient.GetContainerReference(BlobProvider.AzureIdGeneratorContainer.ToLower());

            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Off
                });
            }
            container = blobClient.GetContainerReference(BlobProvider.AzureProductContainer.ToLower());
            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }

            blobClient.GetRootContainerReference().CreateIfNotExists();
            blobClient.GetContainerReference("$root").CreateIfNotExists();
            blobClient.GetRootContainerReference().SetPermissions(
            new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });


            var blob = blobClient.GetRootContainerReference().GetBlockBlobReference("crossdomain.xml");
            blob.Properties.ContentType = "text/xml";
            var bytes = Encoding.ASCII.GetBytes(@"<?xml version=""1.0"" encoding=""utf-8""?>  
                    <cross-domain-policy>  
                        <allow-access-from domain=""http://*.multimicloud.com"" />                           
                        <allow-access-from domain=""http://*.cloudapp.net"" />  
                    </cross-domain-policy>");
            using (var ms = new MemoryStream(bytes, false))
            {
                blob.UploadFromStream(ms);
            }
        }

        private static void CreateQueues(CloudQueueClient queueClient)
        {
            var queue = queueClient.GetQueueReference(QueueName.QueueName2.ToLower());
            var downloadContentFromUrlQueue = queueClient.GetQueueReference(QueueName.DownloadContentFromUrl.ToLower());
            var downloadContentFromUrlQueuePahse2 = queueClient.GetQueueReference(QueueName.DownloadContentFromUrlPhase2.ToLower());
            var mailQueue2 = queueClient.GetQueueReference(QueueName.NewMailQueueName.ToLower());
            var transactionQueue = queueClient.GetQueueReference(QueueName.UpdateDomainQueueName.ToLower());
            var OrderQueue = queueClient.GetQueueReference(QueueName.OrderQueueName.ToLower());

            queue.CreateIfNotExists();
            mailQueue2.CreateIfNotExists();
            transactionQueue.CreateIfNotExists();
            downloadContentFromUrlQueue.CreateIfNotExists();
            downloadContentFromUrlQueuePahse2.CreateIfNotExists();
            OrderQueue.CreateIfNotExists();

        }

        private static void CreateTables(CloudTableClient tableClient)
        {
            var table = tableClient.GetTableReference(TableProvider.UserRequests);
            var table2 = tableClient.GetTableReference(TableProvider.FilterWords);
            table.CreateIfNotExists();
            table2.CreateIfNotExists();

        }
        #endregion


    }

    public class LocalResource
    {

        public string LocalResourcePath { get; set; }
        public int LocalResourceSizeInMegaBytes { get; set; }
    }
}
