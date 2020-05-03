namespace Cloudents.Command.Command
{
    public class SendChatTextMessageCommand : ICommand
    {
        public SendChatTextMessageCommand(string message, long userSendingId, long toUsersId, string? identifier)
        {
            Message = message;
            UserSendingId = userSendingId;
            ToUsersId = toUsersId;
            Identifier = identifier;
        }

        public string Message { get; }
        public long UserSendingId { get; }


        public long ToUsersId { get; }
        public string? Identifier { get; }
    }
}