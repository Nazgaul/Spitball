using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class InviteToSystemCommand : ICommand
    {
        public InviteToSystemCommand(long senderId, IList<string> recipients)
        {
            SenderId = senderId;
            Recipients = recipients;
        }

        public long SenderId { get; private set; }
        public IList<string> Recipients { get; private set; }
    }
}
