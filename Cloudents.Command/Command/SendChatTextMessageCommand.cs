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

        public SendChatTextMessageCommand(string message, long userSendingId, long toUsersId)
        {
            Message = message;
            UserSendingId = userSendingId;
            ToUsersId = toUsersId;
        }

        // public Guid? ChatRoomId { get;  }

        public string Message { get; }
        public long UserSendingId { get; }

        // public string Blob { get; }

        public long ToUsersId { get; }
    }
}