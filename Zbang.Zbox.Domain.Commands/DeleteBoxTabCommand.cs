using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteBoxTabCommand : ICommand
    {
        public DeleteBoxTabCommand(long userId, Guid tabId)
        {
            TabId = tabId;
            UserId = userId;
        }

        public long UserId { get; private set; }
        public Guid TabId { get; private set; }
    }
}
