using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ShareBoxFacebookCommand : ICommand
    {
        public ShareBoxFacebookCommand(long senderId, long recipientFacebookUserId,
             long boxId, Guid id, string facebookUserName)
        {
            FacebookUserName = facebookUserName;
            Id = id;
            SenderId = senderId;
            FacebookUserId = recipientFacebookUserId;
            BoxId = boxId;
        }
        public long SenderId { get; private set; }
        public long FacebookUserId { get; private set; }

        public long BoxId { get; private set; }

        public Guid Id { get; private set; }


        public string FacebookUserName { get; private set; }
    }
}
