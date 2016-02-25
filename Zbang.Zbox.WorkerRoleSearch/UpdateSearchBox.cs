using System;
using System.Linq;
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
        private readonly IZboxWorkerRoleService m_ZboxWriteService;

        private const string PrefixLog = "Search Box";
        public UpdateSearchBox(IZboxReadServiceWorkerRole zboxReadService, 
            IBoxWriteSearchProvider2 boxSearchProvider, 
            IZboxWorkerRoleService zboxWriteService)
        {
            m_ZboxReadService = zboxReadService;
            m_BoxSearchProvider = boxSearchProvider;
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


        public async Task Run(CancellationToken cancellationToken)
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
            m_Interval = Math.Min(MaxInterval, m_Interval * 2);
            await Task.Delay(TimeSpan.FromSeconds(m_Interval), cancellationToken);

        }

        private async Task<bool> UpdateBox(int instanceId, int instanceCount)
        {
            var updates = await m_ZboxReadService.GetBoxDirtyUpdates(instanceId, instanceCount, 100);
            if (updates.BoxesToUpdate.Any() || updates.BoxesToDelete.Any())
            {
                TraceLog.WriteInfo(PrefixLog, string.Format("box updating {0} deleting {1}", updates.BoxesToUpdate.Count(),
                    updates.BoxesToDelete.Count()));
                var isSuccess =
                    await m_BoxSearchProvider.UpdateDataAsync(updates.BoxesToUpdate, updates.BoxesToDelete);
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
