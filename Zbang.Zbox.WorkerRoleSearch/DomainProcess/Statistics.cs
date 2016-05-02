using System.Collections.Generic;
using System.Linq;
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
        private readonly IZboxWorkerRoleService m_ZboxWriteService;

        public Statistics(IZboxWorkerRoleService zboxService)
        {
            m_ZboxWriteService = zboxService;

        }
        public Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token)
        {
            var parameters = data as StatisticsData4;
            if (parameters == null) return Infrastructure.Extensions.TaskExtensions.CompletedTaskTrue;

            IEnumerable<StatisticItemData> itemsIdData = null;

            if (parameters.ItemsIds != null)
            {
                itemsIdData = parameters.ItemsIds.Select(
                    s => new StatisticItemData {ItemId = s.Id, Action = (StatisticsAction) s.Action});
            }

            var command = new UpdateStatisticsCommand(itemsIdData,
                parameters.UserId, parameters.StatTime);
            m_ZboxWriteService.Statistics(command);

            return Infrastructure.Extensions.TaskExtensions.CompletedTaskTrue;
        }
    }
}
