using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Storage
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
                System.IO.Directory.CreateDirectory(LocalResource.LocalResourcePath);
            }
        }
        private static void ConfigureStorageAccount()
        {
            try
            {
                _cloudStorageAccount = CloudStorageAccount.Parse(ConfigFetcher.Fetch("StorageConnectionString"));
            }
            catch (ConfigurationErrorsException)
            {
                _cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            }
            //if (RoleEnvironment.IsAvailable)
            //{
            //    StorageCredentials x = new StorageCredentials();

            //    //Microsoft.WindowsAzure.Storage.Auth.StorageCredentials x = new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
            //    //CloudStorageAccount.SetConfigurationSettingPublisher(
            //    //    (configName, configSetter) => configSetter(RoleEnvironment.GetConfigurationSettingValue(configName)));

            //    _cloudStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            //}
            //else
            //{
            //    //_cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=zboxstorage;AccountKey=HQQ2v9EJ0E+7WpkraKJwGyQ7pZ/yXK6YclCeA3e4bki1GnQoTJSNVXDtBZa/5tuEMgzczqgrH9VztfFaNxyiiw==");
            //   _cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            //}
            // not need every time
            CreateBlobStorages(_cloudStorageAccount.CreateCloudBlobClient());
            CreateQueues(_cloudStorageAccount.CreateCloudQueueClient());
            CreateTables(_cloudStorageAccount.CreateCloudTableClient());

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

            blobClient.GetRootContainerReference().CreateIfNotExists();
            blobClient.GetContainerReference("$root").CreateIfNotExists();
            blobClient.GetRootContainerReference().SetPermissions(
            new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });


            var blob = blobClient.GetRootContainerReference().GetBlockBlobReference("crossdomain.xml");
            blob.Properties.ContentType = "text/xml";
            var bytes = System.Text.Encoding.ASCII.GetBytes(@"<?xml version=""1.0"" encoding=""utf-8""?>  
                    <cross-domain-policy>  
                        <allow-access-from domain=""http://*.multimicloud.com"" />                           
                        <allow-access-from domain=""http://*.cloudapp.net"" />  
                    </cross-domain-policy>");
            using (var ms = new System.IO.MemoryStream(bytes, false))
            {
                blob.UploadFromStream(ms);
            }
        }

        private static void CreateQueues(CloudQueueClient queueClient)
        {
            var queue = queueClient.GetQueueReference(QueueProvider.QueueName.ToLower());
            var downloadContentFromUrlQueue = queueClient.GetQueueReference(QueueProvider.DownloadContentFromUrl.ToLower());
            var downloadContentFromUrlQueuePahse2 = queueClient.GetQueueReference(QueueProvider.DownloadContentFromUrlPahse2.ToLower());
            var mailQueue2 = queueClient.GetQueueReference(QueueProvider.NewMailQueueName.ToLower());
            var transactionQueue = queueClient.GetQueueReference(QueueProvider.UpdateDomainQueueName.ToLower());

            queue.CreateIfNotExists();
            mailQueue2.CreateIfNotExists();
            transactionQueue.CreateIfNotExists();
            downloadContentFromUrlQueue.CreateIfNotExists();
            downloadContentFromUrlQueuePahse2.CreateIfNotExists();

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
