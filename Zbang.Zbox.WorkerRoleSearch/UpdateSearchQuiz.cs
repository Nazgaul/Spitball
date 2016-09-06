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
                    //var retVal = await UpdateQuizAsync(index, count);
                    //if (!retVal)
                    //{
                    //    await SleepAndIncreaseIntervalAsync(cancellationToken);
                    //}
                    //else
                    //{
                    //    m_Interval = MinInterval;
                    //}
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(ex);
                }
            }
            TraceLog.WriteError("On finish run");
        }

      

        //private int m_Interval = MinInterval;
        //private const int MinInterval = 30;
        //private const int MaxInterval = 240;
        //private async Task SleepAndIncreaseIntervalAsync(CancellationToken cancellationToken)
        //{
        //    m_Interval = Math.Min(MaxInterval, m_Interval * 2);
        //    await Task.Delay(TimeSpan.FromSeconds(m_Interval), cancellationToken);

        //}

        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken)
        {
            const int top = 100;
            var updates = await m_ZboxReadService.GetQuizzesDirtyUpdatesAsync(instanceId, instanceCount, top);
            //var updates = new QuizToUpdateSearchDto()
            //{
            //    QuizzesToUpdate = new List<QuizSearchDto>(),
            //    QuizzesToDelete = new long[] { 17153L, 17156L }
            //};
            if (updates.QuizzesToUpdate.Any() || updates.QuizzesToDelete.Any())
            {
                TraceLog.WriteInfo(PrefixLog,
                    $"quiz updating {updates.QuizzesToUpdate.Count()} deleting {updates.QuizzesToDelete.Count()}");

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
            return TimeToSleep.Increase;
        }

        protected override string GetPrefix()
        {
            return PrefixLog;
        }
    }
}
