using System;

namespace Cloudents.Command.StudyRooms
{
    public class CreateStudyRoomSessionCommand : ICommand
    {
        public CreateStudyRoomSessionCommand(Guid studyRoomId, string sessionName, long userId)
        {
            StudyRoomId = studyRoomId;
            SessionName = sessionName;
            UserId = userId;
        }
        public Guid StudyRoomId { get; }

        public string SessionName { get;  }

        public long UserId { get;  }
    }
}
