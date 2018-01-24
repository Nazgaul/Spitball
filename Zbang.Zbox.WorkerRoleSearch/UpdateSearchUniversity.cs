using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Search;
using Cloudents.Core.Interfaces;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearchUniversity : UpdateSearch, IJob, IFileProcess
    {
        private readonly IZboxReadServiceWorkerRole _zboxReadService;
        private readonly IUniversityWriteSearchProvider2 _universitySearchProvider;
        private readonly ILogger _logger;

        private readonly IZboxWorkerRoleService _zboxWriteService;

        public UpdateSearchUniversity(IZboxReadServiceWorkerRole zboxReadService, IUniversityWriteSearchProvider2 universitySearchProvider, IZboxWorkerRoleService zboxWriteService, ILogger logger)
        {
            _zboxReadService = zboxReadService;
            _universitySearchProvider = universitySearchProvider;
            _zboxWriteService = zboxWriteService;
            _logger = logger;
            MaxInterval = TimeSpan.FromMinutes(10).TotalSeconds; //Remove once production is up
        }

        public string Name => nameof(UpdateSearchUniversity);
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            _logger.Warning(Name + " index " + index + " count " + count);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await DoProcessAsync(cancellationToken, index, count).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.Exception(ex);
                }
            }
            _logger.Error($"{Name} On finish run");
        }

        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken)
        {
            const int updatesPerCycle = 10;
            var updates = await _zboxReadService.GetUniversitiesDirtyUpdatesAsync(instanceId, instanceCount, updatesPerCycle).ConfigureAwait(false);
            if (!updates.UniversitiesToDelete.Any() && !updates.UniversitiesToUpdate.Any()) return TimeToSleep.Increase;
            var isSuccess =
                await _universitySearchProvider.UpdateDataAsync(updates.UniversitiesToUpdate, updates.UniversitiesToDelete).ConfigureAwait(false);
            if (isSuccess)
            {
                await _zboxWriteService.UpdateSearchUniversityDirtyToRegularAsync(
                    new UpdateDirtyToRegularCommand(
                        updates.UniversitiesToDelete.Union(updates.UniversitiesToUpdate.Select(s => s.Id)))).ConfigureAwait(false);
            }
            if (updates.UniversitiesToUpdate.Count() == updatesPerCycle)
            {
                return TimeToSleep.Min;
            }
            return TimeToSleep.Same;
        }

        public async Task<bool> ExecuteAsync(FileProcess data, CancellationToken token)
        {
            if (!(data is UniversityProcessData parameters)) return true;

            var elem = await _zboxReadService.GetUniversityDirtyUpdatesAsync(parameters.UniversityId).ConfigureAwait(false);

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
                await _universitySearchProvider.UpdateDataAsync(new[] { elem }, null).ConfigureAwait(false);
            if (isSuccess)
            {
                await _zboxWriteService.UpdateSearchUniversityDirtyToRegularAsync(
                    new UpdateDirtyToRegularCommand(new[] { parameters.UniversityId })).ConfigureAwait(false);
            }
            return true;
        }
    }
}
