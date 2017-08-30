using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateStatisticsCommand: ICommandAsync
    {
        
        public UpdateStatisticsCommand(StatisticItemData itemId, long userId)
        {
            ItemId = itemId;
            UserId = userId;
        }
        public StatisticItemData ItemId { get; private set; }
        public long UserId { get; private set; }


       
    }
    public class StatisticItemData
    {
        public long ItemId { get; set; }
        public StatisticsAction Action { get; set; }
    }
}
