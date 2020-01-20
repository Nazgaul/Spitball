
namespace Cloudents.Command.Command.Admin
{
    public class CreateNoteCommand : ICommand
    {
        public CreateNoteCommand(long userId, string text, string adminEmail)
        {
            UserId = userId;
            Text = text;
            AdminEmail = adminEmail;
        }
        public long UserId { get; }
        public string Text { get; }
        public string AdminEmail { get; }
    }
}
