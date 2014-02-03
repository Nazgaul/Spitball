using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateStatisticsCommand: ICommand
    {
        public UpdateStatisticsCommand(IEnumerable<StatisticItemData> itemId,long userId,DateTime statTime)
        {
            ItemId = itemId;
            UserId = userId;
            StatTime = statTime;
        }
        public IEnumerable<StatisticItemData> ItemId { get; private set; }
        public long UserId { get; private set; }
        public DateTime StatTime { get; private set; }


        public class StatisticItemData
        {
            public long ItemId { get; set; }
            public StatisticsAction Action { get; set; }
        }
    }
}
