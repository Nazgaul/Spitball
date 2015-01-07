using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class UpdateSearch : IJob
    {
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
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }

        private async Task ExecuteAsync()
        {
            await UpdateItem();
            await UpdateBox();
            await UpdateUniversity();

        }

        private async Task UpdateItem()
        {
            var updates = await m_ZboxReadService.GetItemDirtyUpdatesAsync();
            while (updates.ItemsToUpdate.Any() || updates.ItemsToDelete.Any())
            {
                var isSuccess =
                    await m_ItemSearchProvider.UpdateData(updates.ItemsToUpdate, updates.ItemsToDelete);
                if (isSuccess)
                {
                    await m_ZboxWriteService.UpdateSearchItemDirtyToRegularAsync(
                        new UpdateDirtyToRegularCommand(
                            updates.ItemsToDelete.Union(updates.ItemsToUpdate.Select(s => s.Id))));
                }
                else
                {
                    break;
                }
                updates = await m_ZboxReadService.GetItemDirtyUpdatesAsync();
            }
        }

        private async Task UpdateBox()
        {
            var updates = await m_ZboxReadService.GetBoxDirtyUpdates();
            while (updates.BoxesToUpdate.Any() || updates.BoxesToDelete.Any())
            {
                var isSuccess =
                    await m_BoxSearchProvider.UpdateData(updates.BoxesToUpdate, updates.BoxesToDelete);
                if (isSuccess)
                {
                    await m_ZboxWriteService.UpdateSearchBoxDirtyToRegularAsync(
                        new UpdateDirtyToRegularCommand(
                            updates.BoxesToDelete.Union(updates.BoxesToUpdate.Select(s => s.Id))));
                }
                else
                {
                    break;
                }
                updates = await m_ZboxReadService.GetBoxDirtyUpdates();
            }

        }



        private async Task UpdateUniversity()
        {
            var updates = await m_ZboxReadService.GetUniversityDirtyUpdates();
            while (updates.UniversitiesToDelete.Any() || updates.UniversitiesToUpdate.Any())
            {
                var isSuccess =
                    await m_UniversitySearchProvider.UpdateData(updates.UniversitiesToUpdate, updates.UniversitiesToDelete);
                if (isSuccess)
                {
                    await m_ZboxWriteService.UpdateSearchUniversityDirtyToRegularAsync(
                        new UpdateDirtyToRegularCommand(
                            updates.UniversitiesToDelete.Union(updates.UniversitiesToUpdate.Select(s => s.Id))));
                }
                else
                {
                    break;
                }
                updates = await m_ZboxReadService.GetUniversityDirtyUpdates();
            }
        }

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
