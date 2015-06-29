using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
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

        private int GetIndex()
        {
            int currentIndex;

            string instanceId = RoleEnvironment.CurrentRoleInstance.Id;
            bool withSuccess = int.TryParse(instanceId.Substring(instanceId.LastIndexOf(".", StringComparison.Ordinal) + 1), out currentIndex);
            if (!withSuccess)
            {
                int.TryParse(instanceId.Substring(instanceId.LastIndexOf("_", StringComparison.Ordinal) + 1), out currentIndex);
            }
            return currentIndex;
        }


        public async Task Run(System.Threading.CancellationToken cancellationToken)
        {

            while (!cancellationToken.IsCancellationRequested)
            {
                var index = GetIndex();
                var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
                try
                {
                    var retVal = await UpdateBox(index, count);
                    if (!retVal)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(ex);
                }
            }
            TraceLog.WriteError("On finish run");
        }


        private async Task<bool> UpdateBox(int instanceId, int instanceCount)
        {
            var updates = await m_ZboxReadService.GetBoxDirtyUpdates(instanceId, instanceCount);
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
