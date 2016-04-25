﻿using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.DomainProcess
{
    public class Statistics : IDomainProcess
    {
        private readonly IZboxWorkerRoleService m_ZboxWriteService;

        public Statistics(IZboxWorkerRoleService zboxService)
        {
            m_ZboxWriteService = zboxService;

        }
        public Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data)
        {
            var parameters4 = data as StatisticsData4;
            if (parameters4 == null) return Infrastructure.Extensions.TaskExtensions.CompletedTaskTrue;
            var command = new UpdateStatisticsCommand(parameters4.ItemsIds.Select(
                s => new StatisticItemData { ItemId = s.Id, Action = (StatisticsAction)s.Action }),
                parameters4.UserId, parameters4.StatTime);
            m_ZboxWriteService.Statistics(command);

            return Infrastructure.Extensions.TaskExtensions.CompletedTaskTrue;
        }
    }
}
