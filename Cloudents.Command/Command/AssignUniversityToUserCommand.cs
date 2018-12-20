using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command
{
    public class AssignUniversityToUserCommand : ICommand
    {
        public AssignUniversityToUserCommand(long userId, string universityName, string country)
        {
            UserId = userId;
            UniversityName = universityName;
            Country = country;
        }

        public long UserId { get; }
        public string UniversityName { get; }

        public string Country { get;  }
    }
}