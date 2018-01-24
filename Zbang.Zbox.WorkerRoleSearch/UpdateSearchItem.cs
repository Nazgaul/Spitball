using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;
using Cloudents.Core.Interfaces;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Queries.Search;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearchItem : UpdateSearch, IJob, IFileProcess
    {
        private readonly IZboxReadServiceWorkerRole _zboxReadService;
        private readonly IZboxWorkerRoleService _zboxWriteService;
        private readonly IFileProcessorFactory _fileProcessorFactory;
        private readonly IItemWriteSearchProvider _itemSearchProvider3;
        private readonly IBlobProvider2<FilesContainerName> _blobProvider;
        private readonly IContentWriteSearchProvider _contentSearchProvider;
        private readonly ILogger _logger;

        public UpdateSearchItem(IZboxReadServiceWorkerRole zboxReadService,
            IZboxWorkerRoleService zboxWriteService,
            IFileProcessorFactory fileProcessorFactory,
            IBlobProvider2<FilesContainerName> blobProvider,
            IItemWriteSearchProvider itemSearchProvider3,
            IContentWriteSearchProvider contentSearchProvider, ILogger logger)
        {
            _zboxReadService = zboxReadService;
            _zboxWriteService = zboxWriteService;
            _fileProcessorFactory = fileProcessorFactory;
            _blobProvider = blobProvider;
            _itemSearchProvider3 = itemSearchProvider3;
            _contentSearchProvider = contentSearchProvider;
            _logger = logger;
        }

        public string Name => nameof(UpdateSearchItem);
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            _logger.Warning("item index " + index + " count " + count);

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
                    _logger.Exception(ex);
                }
            }
            _logger.Error($"{Name} On finish run");
        }

        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken)
        {
            const int top = 10;
            var updates = await _zboxReadService.GetItemsDirtyUpdatesAsync(new SearchItemDirtyQuery(instanceId, instanceCount, top), cancellationToken).ConfigureAwait(false);
            if (!updates.ItemsToUpdate.Any() && !updates.ItemsToDelete.Any()) return TimeToSleep.Increase;
            var tasks = new List<Task>();
            foreach (var elem in updates.ItemsToUpdate)
            {
                tasks.Add(ProcessDocumentAsync(cancellationToken, elem));
            }

            tasks.Add(_itemSearchProvider3.UpdateDataAsync(null, updates.ItemsToDelete.Select(s => s.Id), cancellationToken));
            tasks.Add(_contentSearchProvider.UpdateDataAsync(null, updates.ItemsToDelete, cancellationToken));
            await Task.WhenAll(tasks).ConfigureAwait(true);
            await _zboxWriteService.UpdateSearchItemDirtyToRegularAsync(
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
                PreProcessFile(elem, cancellationToken);
            }
            else
            {
                elem.DocumentContent = null;
            }
            return UploadToAzureSearchAsync(elem, cancellationToken);
        }

        private async Task UploadToAzureSearchAsync(DocumentSearchDto elem, CancellationToken token)
        {
            if (elem.Type == ItemType.Document) //(elem.TypeDocument.ToLower() == "file")
            {
                try
                {
                    await _itemSearchProvider3.UpdateDataAsync(elem, null, token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.Exception(ex, new Dictionary<string, string> { [Name] = "update item" });
                }
            }
        }

        private Processor GetProcessor(DocumentSearchDto msgData)
        {
            Uri uri;
            if (msgData.Type == ItemType.Document)
            {
                uri = _blobProvider.GetBlobUrl(msgData.BlobName);
                return new Processor
                {
                    ContentProcessor = _fileProcessorFactory.GetProcessor(uri),
                    Uri = uri
                };
            }
            if (Uri.TryCreate(msgData.BlobName, UriKind.Absolute, out uri))
            {
                return new Processor
                {
                    ContentProcessor = _fileProcessorFactory.GetProcessor(uri),
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

        private void PreProcessFile(DocumentSearchDto msgData, CancellationToken cancellationToken)
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
                    using (var tokenSource = new CancellationTokenSource())
                    {
                        tokenSource.CancelAfter(timeSpanToWait);
                        var cancelToken =
                            CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token, cancellationToken);
                        var retVal =
                            await processor.ContentProcessor.PreProcessFileAsync(processor.Uri, cancelToken.Token)
                                .ConfigureAwait(false);
                        try
                        {
                            var proxy = await SignalrClient.GetProxyAsync().ConfigureAwait(false);
                            if (proxy != null)
                            {
                                await proxy.Invoke("UpdateThumbnail", msgData.Id, msgData.Course.Id)
                                   .ConfigureAwait(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Exception(ex,
                                new Dictionary<string, string>
                                {
                                    ["ItemId"] = msgData.Id.ToString(),
                                    [Name] = "signalr"
                                });
                        }
                        if (msgData.Type == ItemType.Document)
                        {
                            var command = new UpdateThumbnailCommand(msgData.Id, retVal?.BlobName,
                                msgData.Content, await _blobProvider.MD5Async(msgData.BlobName).ConfigureAwait(false));
                            _zboxWriteService.UpdateThumbnailPicture(command);
                        }
                        // ReSharper disable once AccessToDisposedClosure
                        wait.Set();
                    }
                }
                catch (TaskCanceledException)
                {
                    wait.Set();
                }
                catch (Exception ex)
                {
                    _logger.Exception(ex, new Dictionary<string, string> { ["ItemId"] = msgData.Id.ToString() });

                    // ReSharper disable once AccessToDisposedClosure
                    wait.Set();
                }
            });
            work.Start();
            var signal = wait.WaitOne(timeSpanToWait);
            if (!signal)
            {
                work.Abort();
                _logger.Error("blob url aborting process " + msgData.BlobName);
            }
            wait.Close();
        }

        private readonly TimeSpan _timeToWait = TimeSpan.FromMinutes(double.Parse(ConfigFetcher.Fetch("TimeToExtractText")));
        private string ExtractContentToUploadToSearch(DocumentSearchDto elem, CancellationToken token)
        {
            if (elem.Type != ItemType.Document)
            {
                return null;
            }
            if (elem.PreviewFailed)
            {
                _logger.Info($"{Name} skipping extract content due to preview failed ");
                return null;
            }
            //TraceLog.WriteInfo(PrefixLog, "search processing " + elem);
            try
            {
                var wait = new ManualResetEvent(false);

                var uri = _blobProvider.GetBlobUrl(elem.BlobName);
                var processor = _fileProcessorFactory.GetProcessor(uri);
                if (processor == null) return null;
                string str = null;
                using (var cancellationTokenSource = new CancellationTokenSource(_timeToWait))
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
                                _logger.Exception(ex, new Dictionary<string, string> { ["element"] = elem.ToString() });
                                wait.Set();
                            }
                        });
                    work.Start();
                    var signal = wait.WaitOne(_timeToWait);
                    if (!signal)
                    {
                        work.Abort();
                        _logger.Error("aborting returning null on elem " + elem);
                        return null;
                    }
                    return str;
                }
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, new Dictionary<string, string> { ["element"] = elem.ToString() });
                return null;
            }
        }

        public async Task<bool> ExecuteAsync(FileProcess data, CancellationToken token)
        {
            if (!(data is BoxFileProcessData parameters)) return true;
            try
            {
                var elements =
                    await _zboxReadService.GetItemsDirtyUpdatesAsync(
                        new SearchItemDirtyQuery(parameters.ItemId), token).ConfigureAwait(false);
                var elem = elements.ItemsToUpdate.FirstOrDefault();
                if (elem == null)
                {
                    return true;
                }
                await ProcessDocumentAsync(token, elem).ConfigureAwait(false);
                await _zboxWriteService.UpdateSearchItemDirtyToRegularAsync(
                    new UpdateDirtyToRegularCommand(new[] { parameters.ItemId })).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, new Dictionary<string, string> { [nameof(parameters)] = parameters.ToString() });
                return false;
            }
        }
    }
}
