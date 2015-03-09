﻿using System.Configuration;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;
using Microsoft.WindowsAzure.Storage.Blob;
//using Zbang.Zbox.Infrastructure.Azure.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Zbang.Zbox.Infrastructure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
//using Zbang.Zbox.Infrastructure.Azure.Table;
using System;

namespace Zbang.Zbox.Infrastructure.Azure.Storage
{
    internal static class StorageProvider
    {
        private static CloudStorageAccount _cloudStorageAccount;
        //private static LocalResource _localStorage;

        static StorageProvider()
        {

            ConfigureStorageAccount();
            //ConfigureLocalStorage();
        }
        //private static void ConfigureLocalStorage()
        //{
        //    if (RoleEnvironment.IsAvailable)
        //    {
        //        var azureLocalResource = RoleEnvironment.GetLocalResource("ItemPreviewStorage");
        //        _localStorage = new LocalResource { LocalResourcePath = azureLocalResource.RootPath, LocalResourceSizeInMegaBytes = azureLocalResource.MaximumSizeInMegabytes };
        //    }
        //    else
        //    {
        //        _localStorage = new LocalResource { LocalResourcePath = "c:\\Temp\\Zbox", LocalResourceSizeInMegaBytes = 200 };
        //        Directory.CreateDirectory(LocalResource.LocalResourcePath);
        //    }
        //}
        private static void ConfigureStorageAccount()
        {
            try
            {
                var connectionString = ConfigFetcher.Fetch("StorageConnectionString");
                if (string.IsNullOrEmpty(connectionString))
                {
                    _cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
                    CreateStorage();
                    return;
                }
                _cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                CreateStorage();
            }
            catch (ArgumentNullException ex)
            {
                TraceLog.WriteError("on ConfigureStorageAccount", ex);

            }
        }

        private static void CreateStorage()
        {
            //CreateBlobStorages(_cloudStorageAccount.CreateCloudBlobClient());
            //CreateQueues(_cloudStorageAccount.CreateCloudQueueClient());
            //CreateTables(_cloudStorageAccount.CreateCloudTableClient());
        }

        //internal static LocalResource LocalResource
        //{
        //    get
        //    {
        //        return _localStorage;
        //    }
        //}
        internal static CloudStorageAccount ZboxCloudStorage
        {
            get
            {
                return _cloudStorageAccount;
            }
        }


    }

    public class LocalResource
    {

        public string LocalResourcePath { get; set; }
        public int LocalResourceSizeInMegaBytes { get; set; }
    }
}
