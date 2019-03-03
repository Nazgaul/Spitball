namespace Cloudents.Command.Command
{
    public class ChatAddMessageCommand : ICommand
    {
      
        public ChatAddMessageCommand(string message, long userSendingId, long[] usersId)
        {
            Message = message;
            UserSendingId = userSendingId;
            ToUsersId = usersId;
        }

        public string Message { get; private set; }
        public long UserSendingId { get; private set; }

        public long[] ToUsersId { get; private set; }
    }
}