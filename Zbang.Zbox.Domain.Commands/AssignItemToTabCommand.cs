using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AssignItemToTabCommand : ICommand
    {
        public AssignItemToTabCommand(long itemId, Guid? tabId, long boxId, long userId)
        {
            BoxId = boxId;
            TabId = tabId;
            ItemId = itemId;
            UserId = userId;
        }

        public long ItemId { get; private set; }
        public Guid? TabId { get; private set; }
        public long BoxId { get; private set; }
        public long UserId { get; private set; }

    }
}
