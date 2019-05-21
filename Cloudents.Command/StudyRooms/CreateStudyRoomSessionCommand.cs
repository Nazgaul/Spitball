using System;

namespace Cloudents.Command.StudyRooms
{
    public class CreateStudyRoomSessionCommand : ICommand
    {
        public CreateStudyRoomSessionCommand(Guid studyRoomId,
            bool recordVideo,
            long userId)
        {
            StudyRoomId = studyRoomId;
            RecordVideo = recordVideo;
            UserId = userId;
        }
        public Guid StudyRoomId { get; }

        public bool RecordVideo { get;  }

        public long UserId { get;  }
    }
}
