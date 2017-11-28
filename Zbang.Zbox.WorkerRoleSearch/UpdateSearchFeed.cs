using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Blob;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Qna;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearchFeed : UpdateSearch, IJob
    {
        private readonly ICloudBlockProvider _cloudBlockProvider;
        private readonly IZboxReadServiceWorkerRole _zboxReadService;
        private readonly IFeedWriteSearchProvider _searchProvider;
        private readonly IWatsonExtract _watsonExtractProvider;
        private readonly IZboxWriteService _writeService;
        private readonly ILogger _logger;

        public UpdateSearchFeed(ICloudBlockProvider cloudBlockProvider, IZboxReadServiceWorkerRole zboxReadService, IFeedWriteSearchProvider searchProvider, IWatsonExtract watsonExtractProvider, IZboxWriteService writeService, ILogger logger)
        {
            _cloudBlockProvider = cloudBlockProvider;
            _zboxReadService = zboxReadService;
            _searchProvider = searchProvider;
            _watsonExtractProvider = watsonExtractProvider;
            _writeService = writeService;
            _logger = logger;
        }

        public string Name => nameof(UpdateSearchFeed);
        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken)
        {
            const int size = 100;
            var page = 0;
            var version = await ReadVersionBlobDataAsync(cancellationToken).ConfigureAwait(false);
            FeedToUpdateSearchDto updates;
            do
            {
                updates = await _zboxReadService.GetFeedDirtyUpdatesAsync(version, page, size, cancellationToken)
                    .ConfigureAwait(false);
                if (!updates.Updates.Any() && !updates.Deletes.Any())
                {
                    if (page == 0)
                    {
                        await WriteNextVersionAsync(cancellationToken, version).ConfigureAwait(false);
                        return TimeToSleep.Increase;
                    }
                }
                _logger.Info("Feed search Going to process " + updates.NextVersion);
                foreach (var feed in updates.Updates.Where(w => w.University != null && JaredUniversityIdPilot.Contains(w.University.Id)))
                {
                    await JaredPilotAsync(feed, cancellationToken).Unwrap().ConfigureAwait(false); // otherwise we got race condition
                }

                await _searchProvider.UpdateDataAsync(null, updates.Deletes, cancellationToken).ConfigureAwait(false);
                //await Task.WhenAll(tasks).ConfigureAwait(false);
                page++;
                //version = Math.Max(version.GetValueOrDefault(), updates.NextVersion);
            } while (updates.Updates.Count() == size);

            await WriteNextVersionAsync(cancellationToken, version).ConfigureAwait(false);
            if (page == 1)
            {
                return TimeToSleep.Same;
            }
            return TimeToSleep.Min;
        }

        private async Task WriteNextVersionAsync(CancellationToken cancellationToken, long? version)
        {
            var currentVersionDb = await _zboxReadService.GetTrackingCurrentVersionAsync().ConfigureAwait(false);
            if (currentVersionDb != version)
            {
                await WriteVersionAsync(currentVersionDb, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<Task> JaredPilotAsync(FeedSearchDto elem, CancellationToken cancellationToken)
        {
            if (elem.Language.GetValueOrDefault(Language.Undefined) == Language.Undefined)
            {
                elem.Language = await _watsonExtractProvider.GetLanguageAsync(elem.Content,cancellationToken).ConfigureAwait(false);
            }

            if (elem.Language == Language.EnglishUs && elem.Tags.All(a => a.Type != TagType.Watson))
            {
                var result = await _watsonExtractProvider.GetKeywordAsync(elem.Content, cancellationToken).ConfigureAwait(false);
                if (result != null)
                {
                    var tags = result as IList<string> ?? result.ToList();
                    elem.Tags.AddRange(tags.Select(s => new FeedSearchTag { Name = s }));
                    var z = new AssignTagsToFeedCommand(elem.Id, tags, TagType.Watson);
                    await _writeService.AddItemTagAsync(z).ConfigureAwait(false);
                }
            }
            return _searchProvider.UpdateDataAsync(elem, null, cancellationToken);
        }

        private async Task<long?> ReadVersionBlobDataAsync(CancellationToken cancellationToken)
        {
            var blob = GetBlobVersion();
            try
            {
                var txt = await blob.DownloadTextAsync(cancellationToken).ConfigureAwait(false);
                if (txt == null)
                {
                    return null;
                }
                if (long.TryParse(txt, out long num))
                {
                    return num;
                }
            }
            catch (Microsoft.WindowsAzure.Storage.StorageException)
            {
                return null;
            }
            return null;
        }

        private Task WriteVersionAsync(long version, CancellationToken token)
        {
            var blob = GetBlobVersion();
            return blob.UploadTextAsync(version.ToString(), token);
        }

        private CloudBlockBlob GetBlobVersion()
        {
            var blob = _cloudBlockProvider.GetFile("dbVersion", "zboxIdGenerator".ToLower());
            return blob;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            _logger.Warning($"{Name} index {index} count {count}");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await DoProcessAsync(cancellationToken, index, count).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.Exception(ex);
                }
            }
            _logger.Error($"{Name} on finish run");
        }
    }
}
