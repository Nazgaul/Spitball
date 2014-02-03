using System.Collections.Generic;
using System.Runtime.Serialization;

using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ShareBoxCommand : ICommand
    {
        public ShareBoxCommand(long boxId, long inviteeId, IList<string> recepients)
        {
            InviteeId = inviteeId;
            BoxId = boxId;
            Recepients = recepients;
        }

        public long BoxId { get; private set; }
        public long InviteeId { get; private set; }
        public IList<string> Recepients { get; private set; }
    }
}
