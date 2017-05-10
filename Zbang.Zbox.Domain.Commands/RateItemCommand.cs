using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class RateItemCommand : ICommandAsync
    {
        public RateItemCommand(long itemId, long userId, Guid id, long? boxId = null)
        {
            BoxId = boxId;
            ItemId = itemId;
            UserId = userId;
            Id = id;
        }

       
        public long ItemId { get; private set; }
        public long UserId { get; private set; }


        public Guid Id { get; private set; }

        public long? BoxId { get; private set; }
    }
}
