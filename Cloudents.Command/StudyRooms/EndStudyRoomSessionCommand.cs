using System;

namespace Cloudents.Command.StudyRooms
{
    public class EndStudyRoomSessionCommand : ICommand
    {
        public EndStudyRoomSessionCommand(Guid studyRoomId,  long userId)
        {
            StudyRoomId = studyRoomId;
            UserId = userId;
        }
        public Guid StudyRoomId { get; }


        public long UserId { get; }
    }
}