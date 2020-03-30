namespace Cloudents.Command.Command
{
    public class SendChatTextMessageCommand : ICommand
    {

        //public SendChatTextMessageCommand(string message, long userSendingId, IEnumerable<long> usersId)
        //{
        //    Message = message;
        //    UserSendingId = userSendingId;
        //    ToUsersId = usersId;
        //}

        public SendChatTextMessageCommand(string message, long userSendingId, long toUsersId, string identifier)
        {
            Message = message;
            UserSendingId = userSendingId;
            ToUsersId = toUsersId;
            Identifier = identifier;
        }

        // public Guid? ChatRoomId { get;  }

        public string Message { get; }
        public long UserSendingId { get; }

        // public string Blob { get; }

        public long ToUsersId { get; }
        public string Identifier { get; }
    }
}