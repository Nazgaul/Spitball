using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.DomainProcess
{
    public class Statistics : IDomainProcess
    {
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IShortCodesCache m_ShortToLongCode;

        public Statistics(IZboxWriteService zboxService, IShortCodesCache shortToLongCode)
        {
            m_ZboxWriteService = zboxService;
            m_ShortToLongCode = shortToLongCode;

        }
        public bool Excecute(Infrastructure.Transport.DomainProcess data)
        {
            var parameters4 = data as StatisticsData4;
            if (parameters4 != null)
            {
                var command = new UpdateStatisticsCommand(parameters4.ItemsIds.Select(s => new Zbang.Zbox.Domain.Commands.UpdateStatisticsCommand.StatisticItemData { ItemId = s.Id, Action = (StatisticsAction)s.Action }), parameters4.UserId, parameters4.StatTime);
                m_ZboxWriteService.Statistics(command);
            }

            return true;
        }
    }
}
