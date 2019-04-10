using System;

namespace Cloudents.Command.Command
{
    public class CreateStudyRoomSessionCommand : ICommand
    {
        public CreateStudyRoomSessionCommand(Guid studyRoomId, string sessionName, long tutorId)
        {
            StudyRoomId = studyRoomId;
            SessionName = sessionName;
            TutorId = tutorId;
        }
        public Guid StudyRoomId { get; }

        public string SessionName { get;  }

        public long TutorId { get;  }
    }
}
