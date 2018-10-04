using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class AssignUniversityToUserCommand : ICommand
    {
        public AssignUniversityToUserCommand(long userId, long universityId)
        {
            UserId = userId;
            UniversityId = universityId;
        }

        public long UserId { get; private set; }
        public long UniversityId { get; private set; }
    }
}