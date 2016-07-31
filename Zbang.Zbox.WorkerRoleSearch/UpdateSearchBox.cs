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

       


        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            TraceLog.WriteWarning("box index " + index + " count " + count);
            while (!cancellationToken.IsCancellationRequested)
            {

                try
                {
                    var retVal = await UpdateBoxAsync(index, count);
                    if (!retVal)
                    {
                        await SleepAndIncreaseIntervalAsync(cancellationToken);
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

        public void Stop()
        {
        }

        int m_Interval = MinInterval;
        private const int MinInterval = 30;
        private const int MaxInterval = 240;
        private async Task SleepAndIncreaseIntervalAsync(CancellationToken cancellationToken)
        {
            m_Interval = Math.Min(MaxInterval, m_Interval * 2);
            await Task.Delay(TimeSpan.FromSeconds(m_Interval), cancellationToken);

        }

        private async Task<bool> UpdateBoxAsync(int instanceId, int instanceCount)
        {
            var updates = await m_ZboxReadService.GetBoxDirtyUpdatesAsync(instanceId, instanceCount, 100);
            if (!updates.BoxesToUpdate.Any() && !updates.BoxesToDelete.Any()) return false;
            //TraceLog.WriteInfo(PrefixLog,
            //    $"box updating {updates.BoxesToUpdate.Count()} deleting {updates.BoxesToDelete.Count()}");
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
    }
}
