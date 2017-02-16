using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Ai;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearchUniversity : UpdateSearch, IJob, IFileProcess
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
            MaxInterval = TimeSpan.FromMinutes(10).TotalSeconds; //Remove once production is up
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

        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken)
        {
            const int updatesPerCycle = 10;
            var updates = await m_ZboxReadService.GetUniversitiesDirtyUpdatesAsync(instanceId, instanceCount, updatesPerCycle);
            if (!updates.UniversitiesToDelete.Any() && !updates.UniversitiesToUpdate.Any()) return TimeToSleep.Increase;

            
            //await
            //    m_WithAiProvider.AddUniversitiesEntityAsync(
            //        updates.UniversitiesToUpdate.Select(s => new UniversityEntityDto
            //        {
            //            Id = s.Id,
            //            Name = s.Name,
            //            Extra = s.Extra?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
            //        }), cancellationToken);
            var isSuccess =
                await m_UniversitySearchProvider.UpdateDataAsync(updates.UniversitiesToUpdate, updates.UniversitiesToDelete);
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

        public async Task<bool> ExecuteAsync(FileProcess data, CancellationToken token)
        {
            var parameters = data as UniversityProcessData;
            if (parameters == null) return true;

            var elem = await m_ZboxReadService.GetUniversityDirtyUpdatesAsync(parameters.UniversityId);

            //await
            //    m_WithAiProvider.AddUniversitiesEntityAsync(new[]
            //    {
            //        new UniversityEntityDto
            //        {
            //            Id = elem.Id,
            //            Name = elem.Name,
            //            Extra = elem.Extra?.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
            //        }
            //    }, token);
            var isSuccess =
                await m_UniversitySearchProvider.UpdateDataAsync(new[] { elem }, null);
            if (isSuccess)
            {
                await m_ZboxWriteService.UpdateSearchUniversityDirtyToRegularAsync(
                    new UpdateDirtyToRegularCommand(new[] { parameters.UniversityId }));

            }
            return true;
        }
    }
}
