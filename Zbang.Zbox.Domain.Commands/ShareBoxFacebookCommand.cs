using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ShareBoxFacebookCommand : ICommand
    {
        public ShareBoxFacebookCommand(long senderId, long recepientFacebookUserId,
            string recepientFacebookUserName, string recepientFacebookName, long boxId)
        {
            SenderId = senderId;
            FacebookUserId = recepientFacebookUserId;
            FacebookUserName = recepientFacebookUserName;
            FacebookName = recepientFacebookName;
            BoxId = boxId;
        }
        public long SenderId { get; private set; }
        public long FacebookUserId { get; private set; }
        public string FacebookUserName { get; private set; }
        public string FacebookName { get; private set; }

        public long BoxId { get; private set; }
    }
}
