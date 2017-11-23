using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearchBox : UpdateSearch, IJob, IFileProcess
    {
        private readonly IZboxReadServiceWorkerRole _zboxReadService;
        private readonly IBoxWriteSearchProvider2 _boxSearchProvider;
        private readonly IZboxWorkerRoleService _zboxWriteService;
        private readonly ILogger _logger;

        public UpdateSearchBox(IZboxReadServiceWorkerRole zboxReadService,
            IBoxWriteSearchProvider2 boxSearchProvider,
            IZboxWorkerRoleService zboxWriteService, ILogger logger
/*,  IZboxWriteService writeService*/)
        {
            _zboxReadService = zboxReadService;
            _boxSearchProvider = boxSearchProvider;
            _zboxWriteService = zboxWriteService;
            _logger = logger;
            //_writeService = writeService;
        }

        public string Name => nameof(UpdateSearchBox);
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            _logger.Warning("box index " + index + " count " + count);
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
            const int top = 100;
            var updates = await _zboxReadService.GetBoxesDirtyUpdatesAsync(instanceId, instanceCount, top, cancellationToken).ConfigureAwait(false);
            if (!updates.BoxesToUpdate.Any() && !updates.BoxesToDelete.Any()) return TimeToSleep.Increase;
            // await JaredPilotAsync(updates.BoxesToUpdate.Where(w => w.UniversityId == JaredUniversityIdPilot));

            //await m_WithAiProvider.AddCoursesEntityAsync(
            //     updates.BoxesToUpdate.Where(
            //         w => w.Type == Infrastructure.Enums.BoxType.Academic && w.Name.Length > 2
            //         ).Select(s => s.Name), cancellationToken);
            var isSuccess = await _boxSearchProvider.UpdateDataAsync(updates.BoxesToUpdate, updates.BoxesToDelete).ConfigureAwait(false);
            if (isSuccess)
            {
                await _zboxWriteService.UpdateSearchBoxDirtyToRegularAsync(new UpdateDirtyToRegularCommand(updates.BoxesToDelete.Union(updates.BoxesToUpdate.Select(s => s.Id)))).ConfigureAwait(false);
            }
            if (updates.BoxesToUpdate.Count() == top)
            {
                return TimeToSleep.Min;
            }
            return TimeToSleep.Same;
        }

        public async Task<bool> ExecuteAsync(FileProcess data, CancellationToken token)
        {
            var parameters = data as BoxProcessData;
            if (parameters == null) return true;

            var elem = await _zboxReadService.GetBoxDirtyUpdatesAsync(parameters.BoxId, token).ConfigureAwait(false);
            var isSuccess =
                await _boxSearchProvider.UpdateDataAsync(new[] { elem }, null).ConfigureAwait(false);
            if (isSuccess)
            {
                await _zboxWriteService.UpdateSearchUniversityDirtyToRegularAsync(
                    new UpdateDirtyToRegularCommand(new[] { parameters.BoxId })).ConfigureAwait(false);
            }
            return true;
        }
    }
}
