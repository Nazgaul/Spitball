﻿using System;
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
            TraceLog.WriteWarning("item index " + index + " count " + count);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var retVal = await UpdateUniversity(index, count);
                    if (!retVal)
                    {
                        await SleepAndIncreaseInterval(cancellationToken);
                    }
                    else
                    {
                        m_Interval = MinInterval;
                        TraceLog.WriteInfo("decrease interval in quiz to " + m_Interval);
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
        private const int MinInterval = 15;
        private const int MaxInterval = 90;
        private async Task SleepAndIncreaseInterval(CancellationToken cancellationToken)
        {
            var previous = m_Interval;
            m_Interval = Math.Min(MaxInterval, m_Interval * 2);
            if (previous != m_Interval)
            {
                TraceLog.WriteInfo("increase interval in university to " + m_Interval);
            }
            await Task.Delay(TimeSpan.FromSeconds(m_Interval), cancellationToken);

        }


        private async Task<bool> UpdateUniversity(int instanceId, int instanceCount)
        {
            var updates = await m_ZboxReadService.GetUniversityDirtyUpdates(instanceId, instanceCount);
            if (updates.UniversitiesToDelete.Any() || updates.UniversitiesToUpdate.Any())
            {
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
                return true;
            }
            return false;
        }
    }
}
