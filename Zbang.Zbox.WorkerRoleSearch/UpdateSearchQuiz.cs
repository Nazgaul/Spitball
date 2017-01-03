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
    public class UpdateSearchQuiz : UpdateSearch, IJob
    {
        private readonly IQuizWriteSearchProvider2 m_QuizSearchProvider;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private const string PrefixLog = "Search Quiz";
        public UpdateSearchQuiz(IQuizWriteSearchProvider2 quizSearchProvider, IZboxReadServiceWorkerRole zboxReadService, IZboxWorkerRoleService zboxWriteService)
        {
            m_QuizSearchProvider = quizSearchProvider;
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
        }
        

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            TraceLog.WriteWarning("quiz index " + index + " count " + count);
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
            const int top = 100;
            var updates = await m_ZboxReadService.GetQuizzesDirtyUpdatesAsync(instanceId, instanceCount, top);
            if (!updates.QuizzesToUpdate.Any() && !updates.QuizzesToDelete.Any()) return TimeToSleep.Increase;
            //TraceLog.WriteInfo(PrefixLog,
                //$"quiz updating {updates.QuizzesToUpdate.Count()} deleting {updates.QuizzesToDelete.Count()}");

            var isSuccess =
                await m_QuizSearchProvider.UpdateDataAsync(updates.QuizzesToUpdate, updates.QuizzesToDelete);
            if (isSuccess)
            {
                await m_ZboxWriteService.UpdateSearchQuizDirtyToRegularAsync(
                    new UpdateDirtyToRegularCommand(
                        updates.QuizzesToDelete.Union(updates.QuizzesToUpdate.Select(s => s.Id))));
            }
            if (updates.QuizzesToUpdate.Count() == top)
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
