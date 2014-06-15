using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteItemTabCommand : ICommand
    {
        public DeleteItemTabCommand(long userId, Guid tabId, long boxId)
        {
            TabId = tabId;
            UserId = userId;
            BoxId = boxId;
        }

        public long UserId { get; private set; }
        public Guid TabId { get; private set; }
        public long BoxId { get; private set; }
    }
}
