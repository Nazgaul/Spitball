using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class RateItemCommand : ICommand
    {
        public RateItemCommand(long itemId, long userId, int rate,Guid id, long boxId)
        {
            BoxId = boxId;
            ItemId = itemId;
            UserId = userId;
            Rate = rate;
            Id = id;
        }
        public long ItemId { get; private set; }
        public long UserId { get; private set; }

        public int Rate { get;private set; }

        public Guid Id { get; private set; }

        public long BoxId { get; private set; }
    }
}
