
namespace Cloudents.Command.Command.Admin
{
    public class DeleteGoogleTokenCommand : ICommand
    {
        public DeleteGoogleTokenCommand(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }
    }
}
