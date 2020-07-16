namespace Cloudents.Command.Command
{
    public class SendChatTextMessageCommand : ICommand
    {
        public SendChatTextMessageCommand(string message, long userSendingId,/* long? toUsersId,*/ string? identifier, long? tutorId)
        {
            Message = message;
            UserSendingId = userSendingId;
          //  ToUsersId = toUsersId;
            Identifier = identifier;
            TutorId = tutorId;
        }

        public string Message { get; }
        public long UserSendingId { get; }

        public long? TutorId { get; }

        //public long? ToUsersId { get; }
        public string? Identifier { get; }
    }
}