using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ChangeUserOnlineStatusCommand : ICommand
    {
        public ChangeUserOnlineStatusCommand(long userId, bool online)
        {
            UserId = userId;
            Online = online;
        }

        public long UserId { get; private set; }

        public bool Online { get; private set; }
    }
}
