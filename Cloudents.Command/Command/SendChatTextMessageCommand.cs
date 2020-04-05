using System;

namespace Cloudents.Command.Command
{
    public class SendChatTextMessageCommand : ICommand
    {
        public SendChatTextMessageCommand(string message, long userSendingId, long toUsersId, Guid? chatRoomId)
        {
            Message = message;
            UserSendingId = userSendingId;
            ToUsersId = toUsersId;
            ChatRoomId = chatRoomId;
        }

         public Guid? ChatRoomId { get;  }

        public string Message { get; }
        public long UserSendingId { get; }

        

        public long ToUsersId { get; }
    }

    public class SendChatTextMessageCommandResult : ICommandResult
    {
        public SendChatTextMessageCommandResult(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}