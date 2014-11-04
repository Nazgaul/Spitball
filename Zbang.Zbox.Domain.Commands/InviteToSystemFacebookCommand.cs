using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class InviteToSystemFacebookCommand : ICommand
    {

        public InviteToSystemFacebookCommand(long senderId, long recipientFacebookUserId,
            Guid id, string facebookUserName)
        {
            FacebookUserName = facebookUserName;
            Id = id;
            SenderId = senderId;
            FacebookUserId = recipientFacebookUserId;
            //FacebookName = recepientFacebookName;

        }


        public long SenderId { get; private set; }
        public long FacebookUserId { get; private set; }

        public Guid Id { get; private set; }
        //public string FacebookName { get; private set; }



        public string FacebookUserName { get; private set; }
    }
}
