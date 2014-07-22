using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ShareBoxCommand : ICommand
    {
        public ShareBoxCommand(long boxId, long inviteeId, IList<string> recipients)
        {
            InviteeId = inviteeId;
            BoxId = boxId;
            Recipients = recipients;
        }

        public long BoxId { get; private set; }
        public long InviteeId { get; private set; }
        public IList<string> Recipients { get; private set; }
    }
}
