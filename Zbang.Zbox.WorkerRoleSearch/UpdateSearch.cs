using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearch : IJob
    {
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;


        private readonly IZboxWorkerRoleService m_ZboxWriteService;


        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private readonly IItemWriteSearchProvider3 m_ItemSearchProvider3;
        private readonly ICloudBlockProvider m_BlobProvider;

        private readonly Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient m_BlobClient;

        private const string PrefixLog = "Search Item";

        public UpdateSearch(IZboxReadServiceWorkerRole zboxReadService,
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


        public async Task Run(CancellationToken cancellationToken)
        {
            var index = GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            TraceLog.WriteWarning("item index " + index + " count " + count);

            while (!cancellationToken.IsCancellationRequested)
            {

                try
                {
                    var itemUpdate = await UpdateItem(index, count);
                    if (!itemUpdate)
                    {
                        await SleepAndIncreaseInterval(cancellationToken);
                    }
                    else
                    {
                        m_Interval = MinInterval;
                    }
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(ex);
                }
            }
            TraceLog.WriteError("On finish run");
        }
        int m_Interval = MinInterval;
        private const int MinInterval = 5;
        private const int MaxInterval = 240;
        private async Task SleepAndIncreaseInterval(CancellationToken cancellationToken)
        {
            var previous = m_Interval;
            m_Interval = Math.Min(MaxInterval, m_Interval * 2);
            if (previous != m_Interval)
            {
                TraceLog.WriteInfo("increase interval in item to " + m_Interval);
            }
            await Task.Delay(TimeSpan.FromSeconds(m_Interval), cancellationToken);

        }

        private int GetIndex()
        {
            int currentIndex;

            string instanceId = RoleEnvironment.CurrentRoleInstance.Id;
            bool withSuccess = int.TryParse(instanceId.Substring(instanceId.LastIndexOf(".", StringComparison.Ordinal) + 1), out currentIndex);
            if (!withSuccess)
            {
                int.TryParse(instanceId.Substring(instanceId.LastIndexOf("_", StringComparison.Ordinal) + 1), out currentIndex);
            }
            return currentIndex;
        }





        private async Task<bool> UpdateItem(int instanceId, int instanceCount)
        {
            //var updates = new ItemToUpdateSearchDto
            //{
            //    ItemsToUpdate = new List<ItemSearchDto>
            //    {
            //        new ItemSearchDto {Id = 291153, BlobName = "12e5fe33-aca7-4aee-b194-2c99a0739d04.docx"}
            //    },
            //    ItemsToDelete = new List<long>()

            //};
            var updates = await m_ZboxReadService.GetItemDirtyUpdatesAsync(instanceId, instanceCount);
            if (!updates.ItemsToUpdate.Any() && !updates.ItemsToDelete.Any()) return false;
            foreach (var elem in updates.ItemsToUpdate)
            {
                PreProcessFile(elem);
                elem.Content = ExtractContentToUploadToSearch(elem);

            }
            var isSuccess2 =
                await m_ItemSearchProvider3.UpdateData(updates.ItemsToUpdate, updates.ItemsToDelete);
            if (isSuccess2)
            {
                await m_ZboxWriteService.UpdateSearchItemDirtyToRegularAsync(
                    new UpdateDirtyToRegularCommand(
                        updates.ItemsToDelete.Union(updates.ItemsToUpdate.Select(s => s.Id))));
            }
            return true;
        }



        private void PreProcessFile(ItemSearchDto msgData)
        {
            var blob = m_BlobProvider.GetFile(msgData.BlobName);
            var processor = m_FileProcessorFactory.GetProcessor(blob.Uri);
            if (processor == null) return;
            var previewContainer = m_BlobClient.GetContainerReference(BlobProvider.AzurePreviewContainer.ToLower());
            var blobInPreview = previewContainer.GetBlockBlobReference(msgData.BlobName + ".jpg");
            if (blobInPreview.Exists() && !(processor is WordProcessor))
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
                    tokenSource.CancelAfter(TimeSpan.FromMinutes(29));
                    //some long running method requiring synchronization
                    var retVal = await processor.PreProcessFile(blob.Uri, tokenSource.Token);

                    if (retVal == null)
                    {
                        wait.Set();
                        return;
                    }
                    var oldBlobName = msgData.BlobName;
                    var command = new UpdateThumbnailCommand(msgData.Id, retVal.ThumbnailName, retVal.BlobName,
                        oldBlobName, retVal.FileTextContent);
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
            Boolean signal = wait.WaitOne(TimeSpan.FromMinutes(30));
            if (!signal)
            {
                work.Abort();
                TraceLog.WriteError("blob url aborting process" + msgData.BlobName);
            }
        }

        readonly TimeSpan m_TimeToWait = TimeSpan.FromMinutes(3);
        private string ExtractContentToUploadToSearch(ItemSearchDto elem)
        {
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

                        str = await processor.ExtractContent(blob.Uri, tokenSource.Token);
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
                Boolean signal = wait.WaitOne(m_TimeToWait);
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
