namespace Cloudents.Command.Command.Admin
{
    public class DeleteUserPaymentCommand : ICommand
    {
        public DeleteUserPaymentCommand(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }
    }
}
