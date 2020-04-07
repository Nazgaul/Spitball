using System;

namespace Cloudents.Command.StudyRooms
{
    public class EnterStudyRoomCommand : ICommand
    {
        public EnterStudyRoomCommand(Guid studyRoomId, long userId)
        {
            StudyRoomId = studyRoomId;
            UserId = userId;
        }

        public Guid StudyRoomId { get; }
        public long UserId { get;  }
    }
}