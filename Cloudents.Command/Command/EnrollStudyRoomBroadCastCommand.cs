using System;

namespace Cloudents.Command.Command
{
    public class EnrollStudyRoomBroadCastCommand : ICommand
    {
        public EnrollStudyRoomBroadCastCommand(long userId, Guid studyRoomId)
        {
            UserId = userId;
            StudyRoomId = studyRoomId;
        }

        public long UserId { get;  }

        public Guid StudyRoomId { get; }

        
    }
}