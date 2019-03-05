namespace Cloudents.Command.Command
{
    public class SendMessageCommand : ICommand
    {
      
        public SendMessageCommand(string message, long userSendingId, long[] usersId)
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