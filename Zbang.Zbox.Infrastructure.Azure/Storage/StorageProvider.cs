using System.IO;
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
using System.Linq;
using System.Reflection;

namespace Zbang.Zbox.Infrastructure.Azure.Storage
{
    internal static class StorageProvider
    {
        static StorageProvider()
        {
            ConfigureStorageAccount();
            try
            {
                ConfigureLocalStorage();
            }
            catch { }
        }

        private static void ConfigureLocalStorage()
        {
            if (RoleEnvironment.IsAvailable)
            {
                var azureLocalResource = RoleEnvironment.GetLocalResource("ItemPreviewStorage");
                LocalResource = new LocalResource { LocalResourcePath = azureLocalResource.RootPath, LocalResourceSizeInMegaBytes = azureLocalResource.MaximumSizeInMegabytes };
            }
            else
            {
                LocalResource = new LocalResource { LocalResourcePath = "c:\\Temp\\Zbox", LocalResourceSizeInMegaBytes = 200 };
                Directory.CreateDirectory(LocalResource.LocalResourcePath);
            }
        }

        private static void ConfigureStorageAccount()
        {
            try
            {
                var connectionString = ConfigFetcher.Fetch("StorageConnectionString");
                if (string.IsNullOrEmpty(connectionString))
                {
                    ZboxCloudStorage = CloudStorageAccount.DevelopmentStorageAccount;
                    //CreateStorage();
                    return;
                }
                ZboxCloudStorage = CloudStorageAccount.Parse(connectionString);
               // CreateStorage();
            }
            catch (ArgumentNullException ex)
            {
                TraceLog.WriteError("on ConfigureStorageAccount", ex);
            }
        }

        private static void CreateStorage()
        {
            CreateBlobStorage(ZboxCloudStorage.CreateCloudBlobClient());
            CreateQueues(ZboxCloudStorage.CreateCloudQueueClient());
            CreateTables(ZboxCloudStorage.CreateCloudTableClient());
        }

        internal static LocalResource LocalResource { get; private set; }

        internal static CloudStorageAccount ZboxCloudStorage { get; private set; }

        #region CreateStorage
        private static void CreateBlobStorage(CloudBlobClient blobClient)
        {
            var container = blobClient.GetContainerReference(BlobProvider.AzureBlobContainer.ToLower());
            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Off
                });
            }
            container = blobClient.GetContainerReference(BlobProvider.AzurePreviewContainer.ToLower());
            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
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
            container = blobClient.GetContainerReference(BlobProvider.AzureQuizContainer.ToLower());
            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
            container = blobClient.GetContainerReference(BlobProvider.AzureChatContainer.ToLower());
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
            container = blobClient.GetContainerReference(BlobProvider.AzureFaqContainer.ToLower());
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
        }

        private static void CreateQueues(CloudQueueClient queueClient)
        {
            var fieldInfos = typeof(QueueName).GetFields(BindingFlags.Public
            | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (var field in fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList())
            {
               var queue =  queueClient.GetQueueReference(field.GetValue(null).ToString().ToLower());
                queue.CreateIfNotExists();
            }
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
