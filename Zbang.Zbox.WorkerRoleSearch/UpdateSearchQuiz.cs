using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearchQuiz : IJob
    {
        private readonly IQuizWriteSearchProvider m_QuizSearchProviderOld;
        private readonly IQuizWriteSearchProvider2 m_QuizSearchProvider;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private const string PrefixLog = "Search Quiz";
        public UpdateSearchQuiz(IQuizWriteSearchProvider quizSearchProviderOld, IQuizWriteSearchProvider2 quizSearchProvider, IZboxReadServiceWorkerRole zboxReadService, IZboxWorkerRoleService zboxWriteService)
        {
            m_QuizSearchProviderOld = quizSearchProviderOld;
            m_QuizSearchProvider = quizSearchProvider;
            m_ZboxReadService = zboxReadService;
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

        public async Task Run(System.Threading.CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var index = GetIndex();
                var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
                try
                {
                    var retVal = await UpdateQuiz(index, count);
                    if (!retVal)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(ex);
                }
            }
            TraceLog.WriteError("On finish run");
        }

        private async Task<bool> UpdateQuiz(int instanceId, int instanceCount)
        {
            var updates = await m_ZboxReadService.GetQuizzesDirtyUpdatesAsync(instanceId, instanceCount);
            if (updates.QuizzesToUpdate.Any() || updates.QuizzesToDelete.Any())
            {
                TraceLog.WriteInfo(PrefixLog, string.Format("quiz updating {0} deleting {1}", updates.QuizzesToUpdate.Count(),
                    updates.QuizzesToDelete.Count()));

                await m_QuizSearchProviderOld.UpdateData(updates.QuizzesToUpdate, updates.QuizzesToDelete);
                var isSuccess =
                    await m_QuizSearchProvider.UpdateData(updates.QuizzesToUpdate, updates.QuizzesToDelete);
                if (isSuccess)
                {
                    await m_ZboxWriteService.UpdateSearchQuizDirtyToRegularAsync(
                        new UpdateDirtyToRegularCommand(
                            updates.QuizzesToDelete.Union(updates.QuizzesToUpdate.Select(s => s.Id))));
                }
                return true;
            }
            return false;
        }
    }
}
