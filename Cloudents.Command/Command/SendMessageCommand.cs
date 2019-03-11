using System;
using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class SendMessageCommand : ICommand
    {
      
        public SendMessageCommand(string message, long userSendingId, IEnumerable<long> usersId,
            Guid? chatRoomId, string blob)
        {
            Message = message;
            UserSendingId = userSendingId;
            ToUsersId = usersId;
            ChatRoomId = chatRoomId;
            Blob = blob;
        }

        public Guid? ChatRoomId { get;  }

        public string Message { get;  }
        public long UserSendingId { get;  }

        public string Blob { get; }

        public IEnumerable<long> ToUsersId { get; }
    }
}