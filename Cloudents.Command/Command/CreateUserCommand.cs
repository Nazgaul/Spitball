using Cloudents.Core.Entities;
using System;

namespace Cloudents.Command.Command
{
    public class CreateUserCommand : ICommand
    {
        public Guid? UniversityId { get; }

        public CreateUserCommand(User user)
        {
            User = user;
        }

        public CreateUserCommand(User user, Guid? universityId) : this(user)
        {
            UniversityId = universityId;
        }

        public User User { get; }
    }
}