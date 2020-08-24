using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command
{
    public class FollowUserCommand : ICommand
    {
        public FollowUserCommand(long followedId, long followerId)
        {
            FollowedId = followedId;
            FollowerId = followerId;
        }
        public long FollowedId { get;  }
        public long FollowerId { get;  }
    }
}
