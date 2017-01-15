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
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateSearchBox : UpdateSearch, IJob, IFileProcess
    {
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IBoxWriteSearchProvider2 m_BoxSearchProvider;
        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private readonly IWitAi m_WithAiProvider;

        // private const string PrefixLog = "Search Box";
        public UpdateSearchBox(IZboxReadServiceWorkerRole zboxReadService,
            IBoxWriteSearchProvider2 boxSearchProvider,
            IZboxWorkerRoleService zboxWriteService, IWitAi withAiProvider)
        {
            m_ZboxReadService = zboxReadService;
            m_BoxSearchProvider = boxSearchProvider;
            m_ZboxWriteService = zboxWriteService;
            m_WithAiProvider = withAiProvider;
        }




        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var index = RoleIndexProcessor.GetIndex();
            var count = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
            TraceLog.WriteWarning("box index " + index + " count " + count);
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





        //int m_Interval = MinInterval;
        //private const int MinInterval = 30;
        // private const int MaxInterval = 240;
        //private async Task SleepAndIncreaseIntervalAsync(CancellationToken cancellationToken)
        //{
        //    await SleepAsync(cancellationToken);
        //    m_Interval = m_Interval*2;
        //}

        //private async Task SleepAsync(CancellationToken cancellationToken)
        //{
        //    await Task.Delay(TimeSpan.FromSeconds(m_Interval), cancellationToken);
        //}

        protected override async Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken)
        {
            const int top = 100;
            var updates = await m_ZboxReadService.GetBoxesDirtyUpdatesAsync(instanceId, instanceCount, top, cancellationToken);
            if (!updates.BoxesToUpdate.Any() && !updates.BoxesToDelete.Any()) return TimeToSleep.Increase;

            //await m_WithAiProvider.AddCoursesEntityAsync(
            //     updates.BoxesToUpdate.Where(
            //         w => w.Type == Infrastructure.Enums.BoxType.Academic && w.Name.Length > 2
            //         ).Select(s => s.Name), cancellationToken);
            var isSuccess = await m_BoxSearchProvider.UpdateDataAsync(updates.BoxesToUpdate, updates.BoxesToDelete);
            if (isSuccess)
            {
                await m_ZboxWriteService.UpdateSearchBoxDirtyToRegularAsync(new UpdateDirtyToRegularCommand(updates.BoxesToDelete.Union(updates.BoxesToUpdate.Select(s => s.Id))));
            }
            if (updates.BoxesToUpdate.Count() == top)
            {
                return TimeToSleep.Min;
            }
            return TimeToSleep.Same;
        }

        protected override string GetPrefix()
        {
            return "Search Box";
        }

        public async Task<bool> ExecuteAsync(FileProcess data, CancellationToken token)
        {
            var parameters = data as BoxProcessData;
            if (parameters == null) return true;

            var elem = await m_ZboxReadService.GetBoxDirtyUpdatesAsync(parameters.BoxId, token);
            //if (elem.Type == Infrastructure.Enums.BoxType.Academic && elem.Name.Length > 2)
            //    await m_WithAiProvider.AddCoursesEntityAsync(new[]
            //    {
            //        elem.Name
            //    }, token);
                 

            var isSuccess =
                await m_BoxSearchProvider.UpdateDataAsync(new[] { elem }, null);
            if (isSuccess)
            {
                await m_ZboxWriteService.UpdateSearchUniversityDirtyToRegularAsync(
                    new UpdateDirtyToRegularCommand(new[] { parameters.BoxId }));

            }
            return true;
        }
    }
}
