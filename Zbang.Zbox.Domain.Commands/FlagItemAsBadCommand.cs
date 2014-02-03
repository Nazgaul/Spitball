using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class FlagItemAsBadCommand : ICommand
    {
        public FlagItemAsBadCommand(long itemId,long userId)
        {
            ItemId = itemId;
            UserId = userId;
        }

        public long ItemId { get; private set; }
        public long UserId { get; private set; }
    }
}
