using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearchItem : IJob
    {
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private readonly IItemWriteSearchProvider3 m_ItemSearchProvider3;
        private readonly ICloudBlockProvider m_BlobProvider;

        private readonly Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient m_BlobClient;
        private const string PrefixLog = "Search Item";

        public UpdateSearchItem(IZboxReadServiceWorkerRole zboxReadService,
            IZboxWorkerRoleService zboxWriteService,
            IFileProcessorFactory fileProcessorFactory,
            ICloudBlockProvider blobProvider, IItemWriteSearchProvider3 itemSearchProvider3)
        {
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
            m_FileProcessorFactory = fileProcessorFactory;
            m_BlobProvider = blobProvider;
            m_ItemSearchProvider3 = itemSearchProvider3;

            var cloudStorageAccount = CloudStorageAccount.Parse(

                   Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("StorageConnectionString"));

            m_BlobClient = cloudStorageAccount.CreateCloudBlobClient();
        }


        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            TraceLog.WriteWarning("item index " + index + " count " + count);

            while (!cancellationToken.IsCancellationRequested)
            {

                try
                {
                    var itemUpdate = await UpdateItemAsync(index, count);
                    if (!itemUpdate)
                    {
                        await SleepAndIncreaseIntervalAsync(cancellationToken);
                    }
                    else
                    {
                        m_Interval = MinInterval;
                    }
                }
                catch (TaskCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                  
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(ex);
                }
            }
            TraceLog.WriteError("On finish run");
        }

        public void Stop()
        {
        }

        private int m_Interval = MinInterval;
        private const int MinInterval = 5;
        private const int MaxInterval = 240;
        private async Task SleepAndIncreaseIntervalAsync(CancellationToken cancellationToken)
        {
            m_Interval = Math.Min(MaxInterval, m_Interval * 2);
            await Task.Delay(TimeSpan.FromSeconds(m_Interval), cancellationToken);

        }

       





        private async Task<bool> UpdateItemAsync(int instanceId, int instanceCount)
        {
            
            var updates = await m_ZboxReadService.GetItemDirtyUpdatesAsync(instanceId, instanceCount, 10);
            if (!updates.ItemsToUpdate.Any() && !updates.ItemsToDelete.Any()) return false;
            var tasks = new List<Task>();
            foreach (var elem in updates.ItemsToUpdate)
            {
                elem.Content = ExtractContentToUploadToSearch(elem);
                PreProcessFile(elem);
                
                if (elem.Type.ToLower() == "file")
                {
                    tasks.Add(
                        m_ItemSearchProvider3.UpdateDataAsync(
                            new[] {elem}, null));
                }
            }
            await Task.WhenAll(tasks);
            await m_ItemSearchProvider3.UpdateDataAsync(null, updates.ItemsToDelete);
            await m_ZboxWriteService.UpdateSearchItemDirtyToRegularAsync(
                new UpdateDirtyToRegularCommand(
                    updates.ItemsToDelete.Union(updates.ItemsToUpdate.Select(s => s.Id))));
            return true;
        }

        private Processor GetProcessor(ItemSearchDto msgData)
        {
            if (msgData.Type.ToLower() != "file")
            {
                Uri uri;
                if (Uri.TryCreate(msgData.BlobName, UriKind.Absolute, out uri))
                {
                    return new Processor
                    {
                        ContentProcessor = m_FileProcessorFactory.GetProcessor(uri),
                        Uri = uri
                    };

                }
            }
            var blob = m_BlobProvider.GetFile(msgData.BlobName);
            return new Processor
            {
                ContentProcessor = m_FileProcessorFactory.GetProcessor(blob.Uri),
                Uri = blob.Uri
            };
        }

        private class Processor
        {
            public IContentProcessor ContentProcessor { get; set; }
            public Uri Uri { get; set; }
        }

        private void PreProcessFile(ItemSearchDto msgData)
        {
            var processor = GetProcessor(msgData);
            //var processor = m_FileProcessorFactory.GetProcessor(blob.Uri);
            if (processor.ContentProcessor == null) return;
            var previewContainer = m_BlobClient.GetContainerReference(BlobProvider.AzurePreviewContainer.ToLower());
            var previewBlobName = WebUtility.UrlEncode(msgData.BlobName + ".jpg");
            if (previewBlobName != null && previewBlobName.Length > 260) //The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters.
            {
                return;
            }
            var blobInPreview = previewContainer.GetBlockBlobReference(previewBlobName);
            if (blobInPreview.Exists())
            {
                return;
            }
            //taken from : http://blogs.msdn.com/b/nikhil_agarwal/archive/2014/04/02/10511934.aspx
            var wait = new ManualResetEvent(false);

            var work = new Thread(async () =>
            {
                try
                {
                    var tokenSource = new CancellationTokenSource();
                    tokenSource.CancelAfter(TimeSpan.FromMinutes(10));
                    //some long running method requiring synchronization
                    var retVal = await processor.ContentProcessor.PreProcessFileAsync(processor.Uri, tokenSource.Token);
                    if (retVal == null)
                    {
                        wait.Set();
                        return;
                    }
                    //var oldBlobName = msgData.BlobName;
                    var command = new UpdateThumbnailCommand(msgData.Id, retVal.BlobName,
                        msgData.Content);
                    m_ZboxWriteService.UpdateThumbnailPicture(command);
                    wait.Set();
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("on itemid: " + msgData.Id, ex);
                    wait.Set();
                }
            });
            work.Start();
            var signal = wait.WaitOne(TimeSpan.FromMinutes(10));
            if (!signal)
            {
                work.Abort();
                TraceLog.WriteError("blob url aborting process" + msgData.BlobName);
            }
        }

        private readonly TimeSpan m_TimeToWait = TimeSpan.FromMinutes(3);
        private string ExtractContentToUploadToSearch(ItemSearchDto elem)
        {
            if (elem.Type.ToLower() != "file")
            {
                return null;
            }
            TraceLog.WriteInfo(PrefixLog, "search processing " + elem);
            try
            {
                var wait = new ManualResetEvent(false);

                var blob = m_BlobProvider.GetFile(elem.BlobName);
                var processor = m_FileProcessorFactory.GetProcessor(blob.Uri);
                if (processor == null) return null;
                var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(m_TimeToWait);

                string str = null;

                var work = new Thread(async () =>
                {
                    try
                    {

                        str = await processor.ExtractContentAsync(blob.Uri, tokenSource.Token);
                        if (string.IsNullOrEmpty(str))
                        {
                            wait.Set();
                            return;
                        }
                        var sb = new StringBuilder();
                        int byteCount = 0;
                        var buffer = new char[1];
                        foreach (var ch in str)
                        {
                            if (char.IsSurrogate(ch))
                            {
                                continue;
                            }
                            buffer[0] = ch;
                            byteCount += Encoding.UTF8.GetByteCount(buffer);
                            if (byteCount > 15000000)
                            {
                                // Couldn't add this character. Return its index
                                break;
                            }
                            sb.Append(ch);
                        }
                        str = sb.ToString();
                        wait.Set();
                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError("on elem " + elem, ex);
                    }
                });
                work.Start();
                bool signal = wait.WaitOne(m_TimeToWait);
                if (!signal)
                {
                    work.Abort();
                    TraceLog.WriteError("aborting returning null on elem " + elem);
                    return null;
                }
                return str;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on elem " + elem, ex);
                return null;
            }
        }








    }
}
