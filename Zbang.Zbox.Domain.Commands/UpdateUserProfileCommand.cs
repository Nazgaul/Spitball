using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserProfileCommand : ICommand
    {
        public UpdateUserProfileCommand(long id,
            string firstName,
            string lastName, long universityId)
        {
            UniversityId = universityId;
            Id = id;

            FirstName = firstName;
            LastName = lastName;

        }

        public long Id { get; private set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public long UniversityId { get; private set; }

    }
}
