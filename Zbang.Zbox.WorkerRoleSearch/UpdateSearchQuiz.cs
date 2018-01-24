using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Search;
using Cloudents.Core.Interfaces;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearchQuiz : UpdateSearch, IJob
    {
        private readonly IQuizWriteSearchProvider2 _quizSearchProvider;
        private readonly IZboxReadServiceWorkerRole _zboxReadService;
        private readonly IZboxWorkerRoleService _zboxWriteService;
        private readonly IContentWriteSearchProvider _contentSearchProvider;
        private readonly ILogger _logger;

        public UpdateSearchQuiz(IQuizWriteSearchProvider2 quizSearchProvider, IZboxReadServiceWorkerRole zboxReadService,
            IZboxWorkerRoleService zboxWriteService, IContentWriteSearchProvider contentSearchProvider,
            ILogger logger)
        {
            _quizSearchProvider = quizSearchProvider;
            _zboxReadService = zboxReadService;
            _zboxWriteService = zboxWriteService;
            _contentSearchProvider = contentSearchProvider;
            _logger = logger;
        }

        public string Name => nameof(UpdateSearchQuiz);
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            _logger.Warning("item index " + index + " count " + count);
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
            try
            {
                const int top = 100;
                var updates =
                    await _zboxReadService.GetQuizzesDirtyUpdatesAsync(instanceId, instanceCount, top)
                        .ConfigureAwait(false);
                if (!updates.QuizzesToUpdate.Any() && !updates.QuizzesToDelete.Any()) return TimeToSleep.Increase;

                var isSuccess =
                    await _quizSearchProvider.UpdateDataAsync(updates.QuizzesToUpdate,
                        updates.QuizzesToDelete.Select(s => s.Id)).ConfigureAwait(false);
                await _contentSearchProvider.UpdateDataAsync(null, updates.QuizzesToDelete, cancellationToken)
                    .ConfigureAwait(false);
                if (isSuccess)
                {
                    await _zboxWriteService.UpdateSearchQuizDirtyToRegularAsync(
                            new UpdateDirtyToRegularCommand(
                                updates.QuizzesToDelete.Select(s => s.Id)
                                    .Union(updates.QuizzesToUpdate.Select(s => s.Id))))
                        .ConfigureAwait(false);
                }
                if (updates.QuizzesToUpdate.Count() == top)
                {
                    return TimeToSleep.Min;
                }
                return TimeToSleep.Same;
            }
            catch (Exception ex)
            {
                _logger.Exception(ex);
                return TimeToSleep.Increase;
            }
        }
    }
}
