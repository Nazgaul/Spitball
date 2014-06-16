using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateItemTabCommand : ICommand
    {
        public CreateItemTabCommand(Guid tabId, string name, long boxId, long userId)
        {
            BoxId = boxId;
            Name = name;
            TabId = tabId;
            UserId = userId;
        }

        public Guid TabId { get; private set; }
        public string Name { get; private set; }
        public long BoxId { get; private set; }
        public long UserId { get; private set; }
    }
}
