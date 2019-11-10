using Cloudents.Core.Entities;

namespace Cloudents.Command.Command
{
    public class CreateUserCommand : ICommand
    {
        public string Course { get; }

        public CreateUserCommand(User user)
        {
            User = user;
        }

        public CreateUserCommand(User user, string course) : this(user)
        {
            Course = course;
        }

        public User User { get; }
    }
}