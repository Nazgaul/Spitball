using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ChatCreateRoomCommand : ICommand
    {
        public ChatCreateRoomCommand(IEnumerable<long> userIds, Guid id)
        {
            UserIds = userIds;
            Id = id;
        }

        public IEnumerable<long> UserIds { get; private set; }

        public Guid Id { get; private set; }
    }
}
