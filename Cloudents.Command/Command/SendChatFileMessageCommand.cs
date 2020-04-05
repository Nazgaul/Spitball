using System;
using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class SendChatFileMessageCommand : ICommand
    {
        public SendChatFileMessageCommand(string blob, long userSendingId, IEnumerable<long> usersId , Guid? identifier = null)
        {
            Blob = blob;
            UserSendingId = userSendingId;
            ToUsersId = usersId;
            Blob = blob;
            Identifier = identifier;
        }


        public long UserSendingId { get; }

        public string Blob { get; }
        public Guid? Identifier { get;  }

        public IEnumerable<long> ToUsersId { get; }
    }
}