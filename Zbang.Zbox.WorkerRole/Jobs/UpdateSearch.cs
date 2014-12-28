using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class UpdateSearch : IJob
    {
        private bool m_KeepRunning;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IUniversityWriteSearchProvider2 m_ZboxWriteSearchProvider;

        public UpdateSearch(IZboxReadServiceWorkerRole zboxReadService, IUniversityWriteSearchProvider2 zboxWriteSearchProvider)
        {
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteSearchProvider = zboxWriteSearchProvider;
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
            await m_ZboxWriteSearchProvider.UpdateData(updates, null);


        }

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
