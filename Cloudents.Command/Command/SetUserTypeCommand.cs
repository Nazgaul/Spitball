using Cloudents.Core.Enum;

namespace Cloudents.Command.Command
{
    public class SetUserTypeCommand : ICommand
    {
        public SetUserTypeCommand(long userId, UserType userType)
        {
            UserId = userId;
            UserType = userType;
        }
        public long UserId { get; set; }
        public UserType UserType { get; set; }
    }
}
