using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            int page = 0;
            
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
                        return TimeToSleep.Increase;
                    }
                    return TimeToSleep.Min;
                }
                var tasks = new List<Task>();
                foreach (var feed in updates.Updates.Where(w => w.UniversityId == JaredUniversityIdPilot))
                {
                    tasks.Add(JaredPilotAsync(feed, cancellationToken));
                }

                tasks.Add(m_SearchProvider.UpdateDataAsync(null, updates.Deletes, cancellationToken));
                await Task.WhenAll(tasks).ConfigureAwait(false);
                page++;
                version = Math.Max(version.GetValueOrDefault(), updates.NextVersion);
            } while (updates.Updates.Count() == size);

            await WriteVersionAsync(updates.NextVersion, cancellationToken).ConfigureAwait(false);
            return TimeToSleep.Same;
        }

        private async Task<Task> JaredPilotAsync(FeedSearchDto elem, CancellationToken cancellationToken)
        {
            if (!elem.Language.HasValue)
            {
                elem.Language = await m_WatsonExtractProvider.GetLanguageAsync(elem.Content,cancellationToken).ConfigureAwait(false);
               // var commandLang = new AddLanguageToFeedCommand(elem.Id, elem.Language.GetValueOrDefault());
                //m_WriteService.AddItemLanguage(commandLang);
            }

            if (elem.Language == Infrastructure.Culture.Language.EnglishUs && elem.Tags.All(a => a.Type != TagType.Watson))
            {

                var result = (await m_WatsonExtractProvider.GetKeywordAsync(elem.Content, cancellationToken).ConfigureAwait(false)).ToList();
                elem.Tags.AddRange(result.Select(s => new ItemSearchTag { Name = s }));
                var z = new AssignTagsToFeedCommand(elem.Id, result, TagType.Watson);
                m_WriteService.AddItemTag(z);
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

        private async Task WriteVersionAsync(long version, CancellationToken token)
        {
            var blob = GetBlobVersion();
            await blob.UploadTextAsync(version.ToString(), token).ConfigureAwait(false);
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
