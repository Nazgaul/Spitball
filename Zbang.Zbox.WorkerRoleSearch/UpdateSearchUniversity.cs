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
    public class UpdateSearchUniversity : IJob
    {
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IUniversityWriteSearchProvider2 m_UniversitySearchProvider;


        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private const string PrefixLog = "Search university";

        public UpdateSearchUniversity(IZboxReadServiceWorkerRole zboxReadService, IUniversityWriteSearchProvider2 universitySearchProvider, IZboxWorkerRoleService zboxWriteService)
        {
            m_ZboxReadService = zboxReadService;
            m_UniversitySearchProvider = universitySearchProvider;
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
            TraceLog.WriteWarning("university index " + index + " count " + count);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var retVal = await UpdateUniversityAsync(index, count);
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

        int m_Interval = MinInterval;
        private const int MinInterval = 5;
        private const int MaxInterval = 60;
        private async Task SleepAndIncreaseIntervalAsync(CancellationToken cancellationToken)
        {
            m_Interval = Math.Min(MaxInterval, m_Interval * 2);
            await Task.Delay(TimeSpan.FromSeconds(m_Interval), cancellationToken);
        }


        private async Task<bool> UpdateUniversityAsync(int instanceId, int instanceCount)
        {
            const int updatesPerCycle = 10;
            var updates = await m_ZboxReadService.GetUniversityDirtyUpdates(instanceId, instanceCount, updatesPerCycle);
            if (!updates.UniversitiesToDelete.Any() && !updates.UniversitiesToUpdate.Any()) return false;
            
            TraceLog.WriteInfo(PrefixLog, string.Format("university updating {0} deleting {1}", updates.UniversitiesToUpdate.Count(),
                updates.UniversitiesToDelete.Count()));
            var isSuccess =
                await m_UniversitySearchProvider.UpdateData(updates.UniversitiesToUpdate, updates.UniversitiesToDelete);
            if (isSuccess)
            {
                await m_ZboxWriteService.UpdateSearchUniversityDirtyToRegularAsync(
                    new UpdateDirtyToRegularCommand(
                        updates.UniversitiesToDelete.Union(updates.UniversitiesToUpdate.Select(s => s.Id))));
            }
            return updates.UniversitiesToUpdate.Count() == updatesPerCycle;
        }
    }
}
