using System;

namespace Cloudents.Command.Universities
{
    public class UserJoinUniversityCommand : ICommand
    {
        public UserJoinUniversityCommand(long userId, Guid universityId)
        {
            UserId = userId;
            UniversityId = universityId;
        }

        public long UserId { get; }
        public Guid UniversityId { get; }
    }
}