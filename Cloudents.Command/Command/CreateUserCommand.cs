using Cloudents.Core.Entities;
using System;

namespace Cloudents.Command.Command
{
    public class CreateUserCommand : ICommand
    {
        public Guid? UniversityId { get; }
        public string Course { get; }

        public CreateUserCommand(User user)
        {
            User = user;
        }

        public CreateUserCommand(User user, Guid? universityId, string course) : this(user)
        {
            UniversityId = universityId;
            Course = course;
        }

        public User User { get; }
    }
}