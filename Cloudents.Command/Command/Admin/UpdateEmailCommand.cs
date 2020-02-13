namespace Cloudents.Command.Command.Admin
{
    public class UpdateEmailCommand : ICommand
    {
        public UpdateEmailCommand(long userId, string email)
        {
            UserId = userId;
            Email = email;
        }
        public long UserId { get; set; }
        public string Email { get; set; }
    }
}
