using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class AddUserTagCommand : ICommand
    {
        public AddUserTagCommand(long userId, string tag)
        {
            UserId = userId;
            Tag = tag;
        }

        public long UserId { get; set; }
        public string Tag { get; set; }
    }
}