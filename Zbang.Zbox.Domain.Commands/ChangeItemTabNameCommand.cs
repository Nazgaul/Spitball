using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ChangeItemTabNameCommand : ICommand
    {
        public ChangeItemTabNameCommand(Guid tabId, string newName, long userId, long boxId)
        {
            UserId = userId;
            NewName = newName;
            TabId = tabId;
            BoxId = boxId;
        }

        public Guid TabId { get; private set; }
        public string NewName { get; private set; }
        public long UserId { get; private set; }
        public long BoxId { get; private set; }
    }
}
