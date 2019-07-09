
namespace Cloudents.Command.Command.Admin
{
    public class AdminSendChatTextMessageCommand : ICommand
    {
        public AdminSendChatTextMessageCommand(string message, long userSendingId, long toUsersId)
        {
            Message = message;
            UserSendingId = userSendingId;
            ToUsersId = toUsersId;
        }
        public string Message { get; }
        public long UserSendingId { get; }
        public long ToUsersId { get; }
    }
}
