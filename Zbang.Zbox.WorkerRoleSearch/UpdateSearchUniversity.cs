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
    public class UpdateSearchUniversity : UpdateSearch, IJob
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
        

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            TraceLog.WriteWarning("university index " + index + " count " + count);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await DoProcessAsync(cancellationToken, index, count);
                    
                }
                catch (Exception ex)
                {

                    TraceLog.WriteError(ex);
                }
            }
            TraceLog.WriteError("On finish run");
        }

       

        //private int m_Interval = MinInterval;
        //private const int MinInterval = 5;
        //private const int MaxInterval = 60;
        //private async Task SleepAndIncreaseIntervalAsync(CancellationToken cancellationToken)
        //{
        //    m_Interval = Math.Min(MaxInterval, m_Interval * 2);
        //    await Task.Delay(TimeSpan.FromSeconds(m_Interval), cancellationToken);
        //}


        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount)
        {
            const int updatesPerCycle = 10;
            var updates = await m_ZboxReadService.GetUniversityDirtyUpdatesAsync(instanceId, instanceCount, updatesPerCycle);
            if (!updates.UniversitiesToDelete.Any() && !updates.UniversitiesToUpdate.Any()) return TimeToSleep.Increase;
            
            //TraceLog.WriteInfo(PrefixLog,
            //    $"university updating {updates.UniversitiesToUpdate.Count()} deleting {updates.UniversitiesToDelete.Count()}");
            var isSuccess =
                await m_UniversitySearchProvider.UpdateData(updates.UniversitiesToUpdate, updates.UniversitiesToDelete);
            if (isSuccess)
            {
                await m_ZboxWriteService.UpdateSearchUniversityDirtyToRegularAsync(
                    new UpdateDirtyToRegularCommand(
                        updates.UniversitiesToDelete.Union(updates.UniversitiesToUpdate.Select(s => s.Id))));
            }
            if (updates.UniversitiesToUpdate.Count() == updatesPerCycle)
            {
                return TimeToSleep.Min;
            }
            return TimeToSleep.Same;
        }

        protected override string GetPrefix()
        {
            return PrefixLog;
        }
    }
}
