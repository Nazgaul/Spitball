using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ShareBoxFacebookCommand : ICommand
    {
        public ShareBoxFacebookCommand(long senderId, long recipientFacebookUserId,
             long boxId, string facebookUserName)
        {
            FacebookUserName = facebookUserName;
            SenderId = senderId;
            FacebookUserId = recipientFacebookUserId;
            BoxId = boxId;
        }
        public long SenderId { get; private set; }
        public long FacebookUserId { get; private set; }

        public long BoxId { get; private set; }



        public string FacebookUserName { get; private set; }

        public string Url { get; set; }
    }
}
