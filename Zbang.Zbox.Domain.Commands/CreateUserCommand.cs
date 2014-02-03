using System;
using System.Runtime.Serialization;

using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public abstract class CreateUserCommand : ICommand
    {
        public CreateUserCommand(string emailId, string userName, long? universityId)
        {
            Email = emailId;
            UserName = userName;
            UniversityId = universityId;
        }

        public string Email { get; private set; }

        public string UserName { get; private set; }
        public long?  UniversityId { get; private set; }

        public abstract string CommandResolveName { get; }
    }
}
