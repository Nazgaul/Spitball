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

        public long UserId { get; private set; }

        public Guid StudyRoomId { get; set; }
    }
}