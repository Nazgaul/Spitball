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
        private const int NumberToReSyncWithoutWait = 500;
        private bool m_KeepRunning;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IUniversityWriteSearchProvider2 m_UniversitySearchProvider;
        private readonly IBoxWriteSearchProvider m_BoxSearchProvider;
        private readonly IZboxWriteService m_ZboxWriteService;

        public UpdateSearch(IZboxReadServiceWorkerRole zboxReadService,
            IUniversityWriteSearchProvider2 zboxWriteSearchProvider,
            IZboxWriteService zboxWriteService, IBoxWriteSearchProvider boxSearchProvider)
        {
            m_ZboxReadService = zboxReadService;
            m_UniversitySearchProvider = zboxWriteSearchProvider;
            m_ZboxWriteService = zboxWriteService;
            m_BoxSearchProvider = boxSearchProvider;
        }

        public void Run()
        {
            m_KeepRunning = true;
            while (m_KeepRunning)
            {
                var t = ExecuteAsync();
                t.Wait();
                if (!t.Result)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                }
            }
        }

        private async Task<bool> ExecuteAsync()
        {
            var boxUpdates = await UpdateBox();
            var needToReSyncImmediately = await UpdateUniversity();

            return boxUpdates || needToReSyncImmediately;
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
                    //await m_ZboxWriteService.UpdateSearchDirtyToRegularAsync(
                    //    new UpdateDirtyToRegularCommand(
                    //        updates.UniversitiesToDelete.Union(updates.UniversitiesToUpdate.Select(s => s.Id))));
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
                    await m_ZboxWriteService.UpdateSearchDirtyToRegularAsync(
                        new UpdateDirtyToRegularCommand(
                            updates.UniversitiesToDelete.Union(updates.UniversitiesToUpdate.Select(s => s.Id))));
                }
            }
            return updates.UniversitiesToUpdate.Count() == NumberToReSyncWithoutWait
                || updates.UniversitiesToDelete.Count() == NumberToReSyncWithoutWait;
        }

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
