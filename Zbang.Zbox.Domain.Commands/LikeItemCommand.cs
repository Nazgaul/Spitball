using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class LikeItemCommand : ICommand
    {
        public LikeItemCommand(long userId, long itemId)
        {
            UserId = userId;
            ItemId = itemId;
        }
        public long UserId { get; private set; }
        public long ItemId { get; private set; }

    }
}
