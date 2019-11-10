namespace Cloudents.Command.Command
{
    public class ChangeOnlineStatusCommand : ICommand
    {
        public ChangeOnlineStatusCommand(long userId, bool status)
        {
            UserId = userId;
            Status = status;
        }

        public long UserId { get; }
        public bool Status { get; }
    }
}