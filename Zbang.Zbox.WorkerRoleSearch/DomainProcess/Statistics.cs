using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.DomainProcess
{
    public class Statistics : IDomainProcess
    {
        private readonly IZboxWorkerRoleService _zboxWriteService;

        public Statistics(IZboxWorkerRoleService zboxService)
        {
            _zboxWriteService = zboxService;
        }

        public async Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token)
        {
            var parameters = data as StatisticsData4;
            if (parameters == null) return true;

            if (parameters.ItemsIds != null)
            {
                foreach (var statisticItemData in parameters.ItemsIds)
                {
                    var command = new UpdateStatisticsCommand(new StatisticItemData
                    { ItemId = statisticItemData.Id, Action = (StatisticsAction)statisticItemData.Action }, parameters.UserId);
                  await  _zboxWriteService.StatisticsAsync(command).ConfigureAwait(false);
                }
            }
            if (parameters.ItemsId != null)
            {
                var command = new UpdateStatisticsCommand(new StatisticItemData
                { ItemId = parameters.ItemsId.Id, Action = (StatisticsAction)parameters.ItemsId.Action }, parameters.UserId);
               await _zboxWriteService.StatisticsAsync(command).ConfigureAwait(false);
            }
            return true;
        }
    }
}
