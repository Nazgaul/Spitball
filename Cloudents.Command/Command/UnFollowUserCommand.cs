using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command
{
    public class UnFollowUserCommand : ICommand
    {
        public UnFollowUserCommand(long tutorToFollow, long userId)
        {
            TutorToFollow = tutorToFollow;
            UserId = userId;
        }
        public long TutorToFollow { get; }
        public long UserId { get; }
    }
}
