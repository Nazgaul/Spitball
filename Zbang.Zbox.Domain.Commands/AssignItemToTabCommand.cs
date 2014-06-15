using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AssignItemToTabCommand : ICommand
    {
        public AssignItemToTabCommand(IEnumerable<long> itemId, Guid tabId, long boxId, long userId,bool needDelete)
        {
            BoxId = boxId;
            TabId = tabId;
            ItemsId = itemId.ToList();
            UserId = userId;
            NeedDelete = needDelete;
        }

        public IList<long> ItemsId { get; private set; }
        public Guid TabId { get; private set; }
        public long BoxId { get; private set; }
        public long UserId { get; private set; }

        public bool NeedDelete { get; private set; }
    }
}
