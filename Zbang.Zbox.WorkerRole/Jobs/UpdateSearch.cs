using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class UpdateSearch : IJob
    {
        private const int NumberToReSyncWithoutWait = 500;
        private bool m_KeepRunning;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IUniversityWriteSearchProvider2 m_UniversitySearchProvider;
        private readonly IBoxWriteSearchProvider m_BoxSearchProvider;
        private readonly IItemWriteSearchProvider m_ItemSearchProvider;
        private readonly IZboxWriteService m_ZboxWriteService;

        public UpdateSearch(IZboxReadServiceWorkerRole zboxReadService,
            IUniversityWriteSearchProvider2 zboxWriteSearchProvider,
            IZboxWriteService zboxWriteService, IBoxWriteSearchProvider boxSearchProvider, IItemWriteSearchProvider itemSearchProvider)
        {
            m_ZboxReadService = zboxReadService;
            m_UniversitySearchProvider = zboxWriteSearchProvider;
            m_ZboxWriteService = zboxWriteService;
            m_BoxSearchProvider = boxSearchProvider;
            m_ItemSearchProvider = itemSearchProvider;
        }

        public void Run()
        {
            m_KeepRunning = true;
            while (m_KeepRunning)
            {
                ExecuteAsync().Wait();
            }
        }

        private async Task ExecuteAsync()
        {
            TraceLog.WriteInfo("Updating item");

            var itemUpdate = await UpdateItem();
            TraceLog.WriteInfo("Updating university");
            var universityUpdate = await UpdateUniversity();
            TraceLog.WriteInfo("Updating box");
            var boxUpdate = await UpdateBox();

            if (itemUpdate || boxUpdate || universityUpdate)
            {
                return;
            }
            await Task.Delay(TimeSpan.FromMinutes(1));
        }

        private async Task<bool> UpdateItem()
        {
            var updates = await m_ZboxReadService.GetItemDirtyUpdatesAsync();
            if (updates.ItemsToUpdate.Any() || updates.ItemsToDelete.Any())
            {
                var isSuccess =
                    await m_ItemSearchProvider.UpdateData(updates.ItemsToUpdate, updates.ItemsToDelete);
                if (isSuccess)
                {
                    await m_ZboxWriteService.UpdateSearchItemDirtyToRegularAsync(
                        new UpdateDirtyToRegularCommand(
                            updates.ItemsToDelete.Union(updates.ItemsToUpdate.Select(s => s.Id))));
                }
            }
            return updates.ItemsToUpdate.Count() == NumberToReSyncWithoutWait
              || updates.ItemsToDelete.Count() == NumberToReSyncWithoutWait;
        }

        private async Task<bool> UpdateBox()
        {
            var updates = await m_ZboxReadService.GetBoxDirtyUpdates();
            if (updates.BoxesToUpdate.Any() || updates.BoxesToDelete.Any())
            {
                var isSuccess =
                    await m_BoxSearchProvider.UpdateData(updates.BoxesToUpdate, updates.BoxesToDelete);
                if (isSuccess)
                {
                    await m_ZboxWriteService.UpdateSearchBoxDirtyToRegularAsync(
                        new UpdateDirtyToRegularCommand(
                            updates.BoxesToDelete.Union(updates.BoxesToUpdate.Select(s => s.Id))));
                }
            }
            return updates.BoxesToUpdate.Count() == NumberToReSyncWithoutWait
               || updates.BoxesToDelete.Count() == NumberToReSyncWithoutWait;

        }



        private async Task<bool> UpdateUniversity()
        {
            var updates = await m_ZboxReadService.GetUniversityDirtyUpdates();
            if (updates.UniversitiesToDelete.Any() || updates.UniversitiesToUpdate.Any())
            {
                var isSuccess =
                    await m_UniversitySearchProvider.UpdateData(updates.UniversitiesToUpdate, updates.UniversitiesToDelete);
                if (isSuccess)
                {
                    await m_ZboxWriteService.UpdateSearchUniversityDirtyToRegularAsync(
                        new UpdateDirtyToRegularCommand(
                            updates.UniversitiesToDelete.Union(updates.UniversitiesToUpdate.Select(s => s.Id))));
                }
            }
            return updates.UniversitiesToDelete.Count() == NumberToReSyncWithoutWait
                || updates.UniversitiesToUpdate.Count() == NumberToReSyncWithoutWait;
        }

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
