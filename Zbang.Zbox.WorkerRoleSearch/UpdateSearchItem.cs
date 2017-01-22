using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearchItem : UpdateSearch, IJob, IFileProcess
    {
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private readonly IItemWriteSearchProvider m_ItemSearchProvider3;
        private readonly IBlobProvider2<FilesContainerName> m_BlobProvider;
        private readonly IZboxWriteService m_WriteService;
        private readonly IWatsonExtract m_WatsonExtractProvider;

        private const string PrefixLog = "Search Item";

        public UpdateSearchItem(IZboxReadServiceWorkerRole zboxReadService,
            IZboxWorkerRoleService zboxWriteService,
            IFileProcessorFactory fileProcessorFactory,
            IBlobProvider2<FilesContainerName> blobProvider,
            IItemWriteSearchProvider itemSearchProvider3, IZboxWriteService writeService, IWatsonExtract watsonExtractProvider)
        {
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
            m_FileProcessorFactory = fileProcessorFactory;
            m_BlobProvider = blobProvider;
            m_ItemSearchProvider3 = itemSearchProvider3;
            m_WriteService = writeService;
            m_WatsonExtractProvider = watsonExtractProvider;
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
                    await DoProcessAsync(cancellationToken, index, count);

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


        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken)
        {
            const int top = 10;
            var updates = await m_ZboxReadService.GetItemsDirtyUpdatesAsync(instanceId, instanceCount, top);
            if (!updates.ItemsToUpdate.Any() && !updates.ItemsToDelete.Any()) return TimeToSleep.Increase;
            var tasks = new List<Task>();
            foreach (var elem in updates.ItemsToUpdate)
            {
                elem.Content = ExtractContentToUploadToSearch(elem, cancellationToken);

                if (elem.UniversityId == JaredUniversityIdPilot)
                {

                    tasks.Add(JaredPilotAsync(elem, cancellationToken));
                }
                PreProcessFile(elem);
                tasks.Add(UploadToAzureSearchAsync(elem, cancellationToken));
            }

            tasks.Add(m_ItemSearchProvider3.UpdateDataAsync(null, updates.ItemsToDelete, cancellationToken));
            await Task.WhenAll(tasks);
            await m_ZboxWriteService.UpdateSearchItemDirtyToRegularAsync(
                new UpdateDirtyToRegularCommand(
                    updates.ItemsToDelete.Union(updates.ItemsToUpdate.Select(s => s.Id))));

            if (updates.ItemsToUpdate.Count() == top)
            {
                return TimeToSleep.Min;
            }
            return TimeToSleep.Same;
        }

        private async Task JaredPilotAsync(ItemSearchDto elem, CancellationToken token)
        {
            var result = await m_WatsonExtractProvider.GetConceptAsync(elem.Content, token);

            var z = new AssignTagsToItemCommand(elem.Id, result);
            m_WriteService.AddItemTag(z);

            var command = new UpdateItemCourseTagCommand(elem.Id, elem.BoxName, elem.BoxCode, elem.BoxProfessor);
            m_WriteService.UpdateItemCourseTag(command);
        }

        private async Task UploadToAzureSearchAsync(ItemSearchDto elem, CancellationToken token)
        {


            if (elem.Type.ToLower() == "file")
            {
                try
                {
                    await m_ItemSearchProvider3.UpdateDataAsync(elem, null, token);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError($"Error update Item {ex}");
                }
            }
        }

        protected override string GetPrefix()
        {
            return PrefixLog;
        }

        private Processor GetProcessor(ItemSearchDto msgData)
        {
            Uri uri;
            if (msgData.Type.ToLower() != "file")
            {

                if (Uri.TryCreate(msgData.BlobName, UriKind.Absolute, out uri))
                {
                    return new Processor
                    {
                        ContentProcessor = m_FileProcessorFactory.GetProcessor(uri),
                        Uri = uri
                    };

                }
            }
            uri = m_BlobProvider.GetBlobUrl(msgData.BlobName);
            // var blob = m_BlobProvider.GetFile(msgData.BlobName);
            return new Processor
            {
                ContentProcessor = m_FileProcessorFactory.GetProcessor(uri),
                Uri = uri
            };
        }

        private class Processor
        {
            public IContentProcessor ContentProcessor { get; set; }
            public Uri Uri { get; set; }
        }

        //private void CancelSynchronousProcess(ItemSearchDto msgData, CancellationToken token, Func<  Task> action)
        //{
        //    var wait = new ManualResetEvent(false);
        //    var work = new Thread(async () =>
        //    {
        //        await action();
        //    });
        //    work.Start();
        //    var signal = wait.WaitOne(TimeSpan.FromMinutes(10));
        //    if (!signal)
        //    {
        //        work.Abort();
        //        TraceLog.WriteError("blob url aborting process" + msgData.BlobName);
        //    }
        //}

        private void PreProcessFile(ItemSearchDto msgData)
        {
            var processor = GetProcessor(msgData);
            if (processor.ContentProcessor == null) return;
            //taken from : http://blogs.msdn.com/b/nikhil_agarwal/archive/2014/04/02/10511934.aspx
            var wait = new ManualResetEvent(false);
            var work = new Thread(async () =>
            {
                try
                {
                    var tokenSource = new CancellationTokenSource();
                    tokenSource.CancelAfter(TimeSpan.FromMinutes(10));
                    //some long running method requiring synchronization
                    var retVal =
                    await processor.ContentProcessor.PreProcessFileAsync(processor.Uri, tokenSource.Token);
                    try
                    {
                        var proxy = await SignalrClient.GetProxyAsync();
                        await proxy.Invoke("UpdateThumbnail", msgData.Id, msgData.BoxId);
                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError("on signalr UpdateThumbnail", ex);
                    }
                    var command = new UpdateThumbnailCommand(msgData.Id, retVal?.BlobName,
                        msgData.Content, await m_BlobProvider.Md5Async(msgData.BlobName));
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
        private string ExtractContentToUploadToSearch(ItemSearchDto elem, CancellationToken token)
        {

            if (elem.Type.ToLower() != "file")
            {
                return null;
            }
            //TraceLog.WriteInfo(PrefixLog, "search processing " + elem);
            try
            {
                var wait = new ManualResetEvent(false);

                var uri = m_BlobProvider.GetBlobUrl(elem.BlobName);
                var processor = m_FileProcessorFactory.GetProcessor(uri);
                if (processor == null) return null;
                string str = null;
                var tokenSource =
                    CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource(m_TimeToWait).Token,
                        token);
                var work = new Thread(async () =>
                    {
                        try
                        {

                            str = await processor.ExtractContentAsync(uri, tokenSource.Token);
                            if (string.IsNullOrEmpty(str))
                            {
                                wait.Set();
                                return;
                            }
                            var sb = new StringBuilder();
                            var byteCount = 0;
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
                            wait.Set();
                        }
                    });
                work.Start();
                var signal = wait.WaitOne(m_TimeToWait);
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


        public async Task<bool> ExecuteAsync(FileProcess data, CancellationToken token)
        {
            var parameters = data as BoxFileProcessData;
            if (parameters == null) return true;

            var elem = await m_ZboxReadService.GetItemDirtyUpdatesAsync(parameters.ItemId);
            await UploadToAzureSearchAsync(elem, token);
            await m_ZboxWriteService.UpdateSearchItemDirtyToRegularAsync(
                new UpdateDirtyToRegularCommand(new[] { elem.Id }));
            return true;
        }
    }
}
