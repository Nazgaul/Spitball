using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Queries.Search;
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
        private readonly IContentWriteSearchProvider m_ContentSearchProvider;
        private readonly ILogger m_Logger;

        public UpdateSearchItem(IZboxReadServiceWorkerRole zboxReadService,
            IZboxWorkerRoleService zboxWriteService,
            IFileProcessorFactory fileProcessorFactory,
            IBlobProvider2<FilesContainerName> blobProvider,
            IItemWriteSearchProvider itemSearchProvider3,
            IZboxWriteService writeService,
            IWatsonExtract watsonExtractProvider,
            IContentWriteSearchProvider contentSearchProvider, ILogger logger)
        {
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
            m_FileProcessorFactory = fileProcessorFactory;
            m_BlobProvider = blobProvider;
            m_ItemSearchProvider3 = itemSearchProvider3;
            m_WriteService = writeService;
            m_WatsonExtractProvider = watsonExtractProvider;
            m_ContentSearchProvider = contentSearchProvider;
            m_Logger = logger;
        }

        public string Name => nameof(UpdateSearchItem);
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            m_Logger.Warning("item index " + index + " count " + count);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await DoProcessAsync(cancellationToken, index, count).ConfigureAwait(false);
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
                    m_Logger.Exception(ex);
                }
            }
            m_Logger.Error($"{Name} On finish run");
        }

        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken)
        {
            const int top = 10;
            var result = await m_ZboxReadService.GetItemsDirtyUpdatesAsync(new SearchItemDirtyQuery(instanceId, instanceCount, top), cancellationToken).ConfigureAwait(false);
            var updates = result.result;
            if (!updates.ItemsToUpdate.Any() && !updates.ItemsToDelete.Any()) return TimeToSleep.Increase;
            m_Logger.TrackMetric(Name, result.count);
            var tasks = new List<Task>();
            foreach (var elem in updates.ItemsToUpdate)
            {
                tasks.Add(ProcessDocumentAsync(cancellationToken, elem));
            }

            tasks.Add(m_ItemSearchProvider3.UpdateDataAsync(null, updates.ItemsToDelete.Select(s => s.Id), cancellationToken));
            tasks.Add(m_ContentSearchProvider.UpdateDataAsync(null, updates.ItemsToDelete, cancellationToken));
            await Task.WhenAll(tasks).ConfigureAwait(true);
            await m_ZboxWriteService.UpdateSearchItemDirtyToRegularAsync(
                new UpdateDirtyToRegularCommand(
                    updates.ItemsToDelete.Select(s => s.Id).Union(updates.ItemsToUpdate.Select(s => s.Id)))).ConfigureAwait(false);

            if (updates.ItemsToUpdate.Count() == top)
            {
                return TimeToSleep.Min;
            }
            return TimeToSleep.Same;
        }

        private Task ProcessDocumentAsync(CancellationToken cancellationToken, DocumentSearchDto elem)
        {
            if (string.IsNullOrEmpty(elem.DocumentContent))
            {
                elem.DocumentContent = ExtractContentToUploadToSearch(elem, cancellationToken);
                PreProcessFile(elem);
            }
            else
            {
                elem.DocumentContent = null;
            }
            var t1 = Task.CompletedTask;
            if (elem.University != null && JaredUniversityIdPilot.Contains(elem.University.Id))
            {
                t1 = JaredPilotAsync(elem, cancellationToken);
            }

            var t2 = UploadToAzureSearchAsync(elem, cancellationToken);
            return Task.WhenAll(t1, t2);
        }

        private void ExtractText(DocumentSearchDto elem, CancellationToken token)
        {
            if (string.IsNullOrEmpty(elem.Content))
            {
                elem.DocumentContent = ExtractContentToUploadToSearch(elem, token);
            }
        }

        private async Task JaredPilotAsync(DocumentSearchDto elem, CancellationToken token)
        {
            if (elem.Type == ItemType.Document)
            {
                if (elem.Language.GetValueOrDefault(Language.Undefined) == Language.Undefined)
                {
                    ExtractText(elem, token);
                    var result = await m_WatsonExtractProvider.GetLanguageAsync(elem.Content, token).ConfigureAwait(false);
                    elem.Language = result;
                    if (result != Language.Undefined)
                    {
                        var commandLang = new AddLanguageToDocumentCommand(elem.Id, result);
                        m_WriteService.AddItemLanguage(commandLang);
                    }
                }

                if (elem.Language == Language.EnglishUs && elem.Tags.All(a => a.Type != TagType.Watson))
                {
                    ExtractText(elem, token);
                    var result = await m_WatsonExtractProvider.GetConceptAsync(elem.Content, token).ConfigureAwait(false);
                    if (result != null)
                    {
                        var resultList = result.ToList();
                        elem.Tags.AddRange(resultList.Select(s => new ItemSearchTag { Name = s }));
                        var z = new AssignTagsToDocumentCommand(elem.Id, resultList, TagType.Watson);
                        await m_WriteService.AddItemTagAsync(z).ConfigureAwait(false);
                    }
                }

                await m_ContentSearchProvider.UpdateDataAsync(elem, null, token).ConfigureAwait(false);
            }
        }

        private async Task UploadToAzureSearchAsync(DocumentSearchDto elem, CancellationToken token)
        {
            if (elem.Type == ItemType.Document) //(elem.TypeDocument.ToLower() == "file")
            {
                try
                {
                    await m_ItemSearchProvider3.UpdateDataAsync(elem, null, token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    m_Logger.Exception(ex, new Dictionary<string, string> { { Name, "update item" } });
                }
            }
        }

        private Processor GetProcessor(DocumentSearchDto msgData)
        {
            Uri uri;
            if (msgData.Type == ItemType.Document)
            {
                uri = m_BlobProvider.GetBlobUrl(msgData.BlobName);
                return new Processor
                {
                    ContentProcessor = m_FileProcessorFactory.GetProcessor(uri),
                    Uri = uri
                };
            }
            if (Uri.TryCreate(msgData.BlobName, UriKind.Absolute, out uri))
            {
                return new Processor
                {
                    ContentProcessor = m_FileProcessorFactory.GetProcessor(uri),
                    Uri = uri
                };
            }
            return null;
        }

        private class Processor
        {
            public IContentProcessor ContentProcessor { get; set; }
            public Uri Uri { get; set; }
        }

        private void PreProcessFile(DocumentSearchDto msgData)
        {
            var processor = GetProcessor(msgData);
            if (processor?.ContentProcessor == null) return;
            var timeSpanToWait = TimeSpan.FromMinutes(10);
            //taken from : http://blogs.msdn.com/b/nikhil_agarwal/archive/2014/04/02/10511934.aspx
#pragma warning disable CC0022 // Should dispose object we close the event at the end
            var wait = new ManualResetEvent(false);
#pragma warning restore CC0022 // Should dispose object
            var work = new Thread(async () =>
            {
                try
                {
                    var tokenSource = new CancellationTokenSource();
                    tokenSource.CancelAfter(timeSpanToWait);
                    var retVal =
                        await processor.ContentProcessor.PreProcessFileAsync(processor.Uri, tokenSource.Token)
                            .ConfigureAwait(false);
                    try
                    {
                        var proxy = await SignalrClient.GetProxyAsync().ConfigureAwait(false);
                        if (proxy != null)
                            await proxy.Invoke("UpdateThumbnail", msgData.Id, msgData.Course.Id).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        m_Logger.Exception(ex, new Dictionary<string, string> { { "ItemId", msgData.Id.ToString() }, { Name, "signalr" } });
                    }
                    if (msgData.Type == ItemType.Document)
                    {
                        var command = new UpdateThumbnailCommand(msgData.Id, retVal?.BlobName,
                            msgData.Content, await m_BlobProvider.MD5Async(msgData.BlobName).ConfigureAwait(false));
                        m_ZboxWriteService.UpdateThumbnailPicture(command);
                    }
                    // ReSharper disable once AccessToDisposedClosure
                    wait.Set();
                }
                catch (Exception ex)
                {
                    m_Logger.Exception(ex, new Dictionary<string, string> { { "ItemId", msgData.Id.ToString() } });

                    // ReSharper disable once AccessToDisposedClosure
                    wait.Set();
                }
            });
            work.Start();
            var signal = wait.WaitOne(timeSpanToWait);
            if (!signal)
            {
                work.Abort();
                m_Logger.Error("blob url aborting process " + msgData.BlobName);
            }
            wait.Close();
        }

        private readonly TimeSpan m_TimeToWait = TimeSpan.FromMinutes(double.Parse(ConfigFetcher.Fetch("TimeToExtractText")));
        private string ExtractContentToUploadToSearch(DocumentSearchDto elem, CancellationToken token)
        {
            if (elem.Type != ItemType.Document)
            {
                return null;
            }
            if (elem.PreviewFailed)
            {
                m_Logger.Info($"{Name} skipping extract content due to preview failed ");
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
                using (var cancellationTokenSource = new CancellationTokenSource(m_TimeToWait))
                {
                    var tokenSource =
CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token,
token);
                    var work = new Thread(async () =>
                        {
                            try
                            {
                                str = await processor.ExtractContentAsync(uri, tokenSource.Token).ConfigureAwait(false);
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
                                    buffer[0] = ch;
                                    byteCount += Encoding.UTF8.GetByteCount(buffer);
                                    if (byteCount > 1.55e+7) //limit to azure search
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
                                m_Logger.Exception(ex, new Dictionary<string, string> { { "element", elem.ToString() } });
                                wait.Set();
                            }
                        });
                    work.Start();
                    var signal = wait.WaitOne(m_TimeToWait);
                    if (!signal)
                    {
                        work.Abort();
                        m_Logger.Error("aborting returning null on elem " + elem);
                        return null;
                    }
                    return str;
                }
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex, new Dictionary<string, string> { { "element", elem.ToString() } });
                return null;
            }
        }

        public async Task<bool> ExecuteAsync(FileProcess data, CancellationToken token)
        {
            var parameters = data as BoxFileProcessData;
            if (parameters == null) return true;
            try
            {
                var elements =
                    await m_ZboxReadService.GetItemsDirtyUpdatesAsync(
                        new SearchItemDirtyQuery(parameters.ItemId), token).ConfigureAwait(false);
                var elem = elements.result.ItemsToUpdate.FirstOrDefault();
                if (elem == null)
                {
                    return true;
                }
                await ProcessDocumentAsync(token, elem).ConfigureAwait(false);
                await m_ZboxWriteService.UpdateSearchItemDirtyToRegularAsync(
                    new UpdateDirtyToRegularCommand(new[] { parameters.ItemId })).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex, new Dictionary<string, string> { { nameof(parameters), parameters.ToString() } });
                return false;
            }
        }
    }
}
