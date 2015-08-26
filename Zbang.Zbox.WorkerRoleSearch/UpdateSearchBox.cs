using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            var index = GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            TraceLog.WriteWarning("box index " + index + " count " + count);
            while (!cancellationToken.IsCancellationRequested)
            {
               
                try
                {
                    var retVal = await UpdateBox(index, count);
                    if (!retVal)
                    {
                        await SleepAndIncreaseInterval(cancellationToken);
                    }
                    else
                    {
                        m_Interval = MinInterval;
                        TraceLog.WriteInfo("decrease interval in box to " + m_Interval);
                    }
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(ex);
                }
            }
            TraceLog.WriteError("On finish run");
        }
        int m_Interval = MinInterval;
        private const int MinInterval = 30;
        private const int MaxInterval = 240;
        private async Task SleepAndIncreaseInterval(CancellationToken cancellationToken)
        {
            var previous = m_Interval;
            m_Interval = Math.Min(MaxInterval, m_Interval * 2);
            if (previous != m_Interval)
            {
                TraceLog.WriteInfo("increase interval in box to " + m_Interval);
            }
            await Task.Delay(TimeSpan.FromSeconds(m_Interval), cancellationToken);

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
