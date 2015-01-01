using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class UpdateSearch : IJob
    {
        private bool m_KeepRunning;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IUniversityWriteSearchProvider2 m_ZboxWriteSearchProvider;
        private readonly IZboxWriteService m_ZboxWriteService;

        public UpdateSearch(IZboxReadServiceWorkerRole zboxReadService, IUniversityWriteSearchProvider2 zboxWriteSearchProvider, IZboxWriteService zboxWriteService)
        {
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteSearchProvider = zboxWriteSearchProvider;
            m_ZboxWriteService = zboxWriteService;
        }

        public void Run()
        {
            m_KeepRunning = true;
            while (m_KeepRunning)
            {
                ExecuteAsync().Wait();
                Thread.Sleep(TimeSpan.FromSeconds(30));
            }
        }

        private async Task ExecuteAsync()
        {
            var updates = await m_ZboxReadService.GetUniversityDirtyUpdates();
            if (updates.UniversitiesToDelete.Any() || updates.UniversitiesToUpdate.Any())
            {
                var isSuccess = await m_ZboxWriteSearchProvider.UpdateData(updates.UniversitiesToUpdate, updates.UniversitiesToDelete);
                if (isSuccess)
                {
                    await m_ZboxWriteService.UpdateSearchDirtyToRegularAsync();
                }
            }


        }

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
