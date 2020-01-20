using System;

namespace Cloudents.Command.Command.Admin
{
    public class CreateNoteCommand : ICommand
    {
        public CreateNoteCommand(long userId, string text, Guid adminId)
        {
            UserId = userId;
            Text = text;
            AdminId = adminId;
        }
        public long UserId { get; }
        public string Text { get; }
        public Guid AdminId { get; }
    }
}
