using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearchBox : IJob
    {
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IBoxWriteSearchProvider2 m_BoxSearchProvider;
        private readonly IBoxWriteSearchProviderOld m_BoxSearchProviderOld;
        private readonly IZboxWorkerRoleService m_ZboxWriteService;

        private const string PrefixLog = "Search Box";
        public UpdateSearchBox(IZboxReadServiceWorkerRole zboxReadService, IBoxWriteSearchProvider2 boxSearchProvider, IBoxWriteSearchProviderOld boxSearchProviderOld, IZboxWorkerRoleService zboxWriteService)
        {
            m_ZboxReadService = zboxReadService;
            m_BoxSearchProvider = boxSearchProvider;
            m_BoxSearchProviderOld = boxSearchProviderOld;
            m_ZboxWriteService = zboxWriteService;
        }

        public async Task Run(System.Threading.CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
              var retVal =  await UpdateBox();
                if (!retVal)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
            }
            TraceLog.WriteError("On finish run");
        }


        private async Task<bool> UpdateBox()
        {
            var updates = await m_ZboxReadService.GetBoxDirtyUpdates();
            if (updates.BoxesToUpdate.Any() || updates.BoxesToDelete.Any())
            {
                TraceLog.WriteInfo(PrefixLog, string.Format("box updating {0} deleting {1}", updates.BoxesToUpdate.Count(),
                    updates.BoxesToDelete.Count()));
                await m_BoxSearchProviderOld.UpdateData(updates.BoxesToUpdate, updates.BoxesToDelete);
                var isSuccess =
                    await m_BoxSearchProvider.UpdateData(updates.BoxesToUpdate, updates.BoxesToDelete);
                if (isSuccess)
                {
                    await m_ZboxWriteService.UpdateSearchBoxDirtyToRegularAsync(
                        new UpdateDirtyToRegularCommand(
                            updates.BoxesToDelete.Union(updates.BoxesToUpdate.Select(s => s.Id))));
                }

                return true;
            }
            return false;
        }
    }
}
