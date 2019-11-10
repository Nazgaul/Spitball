using System;

namespace Cloudents.Command.StudyRooms
{
    public class EndStudyRoomSessionCommand : ICommand
    {
        public EndStudyRoomSessionCommand(Guid studyRoomId, long userId)
        {
            StudyRoomId = studyRoomId;
            UserId = userId;
        }
        public Guid StudyRoomId { get; }


        public long UserId { get; }
    }


    public class EndStudyRoomSessionTwilioCommand : ICommand
    {
        public Guid StudyRoomId { get; }
        public string StudyRoomSessionId { get; }

        public EndStudyRoomSessionTwilioCommand(Guid studyRoomId, string studyRoomSessionId)
        {
            StudyRoomId = studyRoomId;
            StudyRoomSessionId = studyRoomSessionId;
        }
    }
}