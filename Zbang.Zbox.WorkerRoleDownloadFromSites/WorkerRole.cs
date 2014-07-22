using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.IO;
using Zbang.Zbox.Infrastructure.Transport;
using Microsoft.WindowsAzure.Storage.Blob;
//using Zbang.Zbox.Infrastructure.Trace;
//using Zbang.Zbox.Infrastructure.Storage;
//using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleDownloadFromSites
{
// ReSharper disable once UnusedMember.Global
    public class WorkerRole : RoleEntryPoint
    {
        private readonly UnityFactory m_Unity;
        public WorkerRole()
        {
            m_Unity = new UnityFactory();
        }
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("Zbang.Zbox.WorkerRoleDownloadFromSites entry point called", "Information");

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("zboxStorage"));

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference("downloadcontentfromurl");
            CloudQueue queueToWorkerRole = queueClient.GetQueueReference("downloadcontentfromurlphase2");
            queueToWorkerRole.CreateIfNotExists();
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            while (true)
            {
                try
                {
                    var messages = queue.GetMessages(32);

                    foreach (var message in messages)
                    {
                        var obj = TransferBytesToObjects(message);


                        var content = DownloadContent(obj.Url);
                        var filesContainer = blobClient.GetContainerReference("zboxfiles");

                        var fileName = Guid.NewGuid().ToString().ToLower() + Path.GetExtension(obj.FileName).ToLower();
                        var blob = filesContainer.GetBlockBlobReference(fileName);

                        blob.UploadFromByteArray(content, 0, content.Length);
                        blob.Properties.CacheControl = "private max-age=604800"; //week
                        blob.SetProperties();
                        obj.BlobUrl = fileName;
                        obj.Size = content.Length;

                        var bytes = TransferObjectToBytes(obj);
                        message.SetMessageContent(bytes);

                        queue.DeleteMessage(message);
                        queueToWorkerRole.AddMessage(new CloudQueueMessage(bytes));
                        //queue.UpdateMessage(message,
                        //    TimeSpan.FromSeconds(0.0),  // Make it visible immediately.
                        //   MessageUpdateFields.Content | MessageUpdateFields.Visibility);

                    }
                }
                catch (Exception ex)
                {
                    Trace.Write("error on download from dp" + ex, "Error");
                }
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }


        }

        private byte[] DownloadContent(string url)
        {
            using (var webClient = new System.Net.WebClient())
            {
                return webClient.DownloadData(url);
            }
        }

        private UrlToDownloadData TransferBytesToObjects(CloudQueueMessage message)
        {
            using (var ms = new MemoryStream(message.AsBytes))
            {
                try
                {
                    return ProtoBuf.Serializer.Deserialize<Zbang.Zbox.Infrastructure.Transport.UrlToDownloadData>(ms);
                }
                catch (ProtoBuf.ProtoException)
                {
                    return null;

                }
            }
        }

        private byte[] TransferObjectToBytes(Zbang.Zbox.Infrastructure.Transport.UrlToDownloadData data)
        {
            using (var m = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(m, data);
                m.Seek(0, SeekOrigin.Begin);
                return m.ToArray();

            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
