﻿using System;
using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class SendChatTextMessageCommand : ICommand
    {
      
        public SendChatTextMessageCommand(string message, long userSendingId, IEnumerable<long> usersId
          )
        {
            Message = message;
            UserSendingId = userSendingId;
            ToUsersId = usersId;
          //  ChatRoomId = chatRoomId;
            //Blob = blob;
        }

       // public Guid? ChatRoomId { get;  }

        public string Message { get;  }
        public long UserSendingId { get;  }

       // public string Blob { get; }

        public IEnumerable<long> ToUsersId { get; }
    }
}