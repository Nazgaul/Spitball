using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Blob;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
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
        private readonly ICloudBlockProvider m_CloudBlockProvider;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IFeedWriteSearchProvider m_SearchProvider;
        private readonly IWatsonExtract m_WatsonExtractProvider;
        private readonly IZboxWriteService m_WriteService;

        public UpdateSearchFeed(ICloudBlockProvider cloudBlockProvider, IZboxReadServiceWorkerRole zboxReadService, IFeedWriteSearchProvider searchProvider, IWatsonExtract watsonExtractProvider, IZboxWriteService writeService)
        {
            m_CloudBlockProvider = cloudBlockProvider;
            m_ZboxReadService = zboxReadService;
            m_SearchProvider = searchProvider;
            m_WatsonExtractProvider = watsonExtractProvider;
            m_WriteService = writeService;
        }

        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken)
        {
            const int size = 100;
            var page = 0;
            
            var version = await ReadVersionBlobDataAsync(cancellationToken).ConfigureAwait(false);
            FeedToUpdateSearchDto updates;
            do
            {

                updates = await m_ZboxReadService.GetFeedDirtyUpdatesAsync(version, page, size, cancellationToken)
                    .ConfigureAwait(false);
                if (!updates.Updates.Any() && !updates.Deletes.Any())
                {
                    if (page == 0)
                    {
                        await WriteNextVersionAsync(cancellationToken, version).ConfigureAwait(false);
                        return TimeToSleep.Increase;
                    }
                    
                }
                TraceLog.WriteInfo("Feed search Going to process " + updates.NextVersion);
                //var tasks = new List<Task>();
                foreach (var feed in updates.Updates.Where(w => w.University != null && JaredUniversityIdPilot.Contains(w.University.Id)))
                {
                    await JaredPilotAsync(feed, cancellationToken).ConfigureAwait(false); // otherwise we got race condition
                }

                await m_SearchProvider.UpdateDataAsync(null, updates.Deletes, cancellationToken).ConfigureAwait(false);
                //await Task.WhenAll(tasks).ConfigureAwait(false);
                page++;
                //version = Math.Max(version.GetValueOrDefault(), updates.NextVersion);
            } while (updates.Updates.Count() == size);

            //var newVersion = version.Value;
            await WriteNextVersionAsync(cancellationToken, version).ConfigureAwait(false);
            if (page == 1)
            {
                return TimeToSleep.Same;
            }
            return TimeToSleep.Min;
        }

        private async Task WriteNextVersionAsync(CancellationToken cancellationToken, long? version)
        {
            var currentVersionDb = await m_ZboxReadService.GetTrackingCurrentVersionAsync().ConfigureAwait(false);
            if (currentVersionDb != version)
            {
                await WriteVersionAsync(currentVersionDb, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<Task> JaredPilotAsync(FeedSearchDto elem, CancellationToken cancellationToken)
        {
            if (elem.Language.GetValueOrDefault(Infrastructure.Culture.Language.Undefined) == Infrastructure.Culture.Language.Undefined)
            {
                elem.Language = await m_WatsonExtractProvider.GetLanguageAsync(elem.Content,cancellationToken).ConfigureAwait(false);
              
            }

            if (elem.Language == Infrastructure.Culture.Language.EnglishUs && elem.Tags.All(a => a.Type != TagType.Watson))
            {

                var result = await m_WatsonExtractProvider.GetKeywordAsync(elem.Content, cancellationToken).ConfigureAwait(false);
                if (result != null)
                {
                    var tags = result as IList<string> ?? result.ToList();
                    elem.Tags.AddRange(tags.Select(s => new FeedSearchTag { Name = s }));
                    var z = new AssignTagsToFeedCommand(elem.Id, tags, TagType.Watson);
                    await m_WriteService.AddItemTagAsync(z).ConfigureAwait(false);
                }
            }
            return m_SearchProvider.UpdateDataAsync(elem, null, cancellationToken);
        }

        protected override string GetPrefix()
        {
            return "Feed";
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
                long num;
                if (long.TryParse(txt, out num))
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
            var blob = m_CloudBlockProvider.GetFile("dbVersion", "zboxIdGenerator".ToLower());
            return blob;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            TraceLog.WriteWarning("box index " + index + " count " + count);
            while (!cancellationToken.IsCancellationRequested)
            {

                try
                {
                    await DoProcessAsync(cancellationToken, index, count).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(ex);
                }
            }
            TraceLog.WriteError("On finish run");
        }


    }
}
