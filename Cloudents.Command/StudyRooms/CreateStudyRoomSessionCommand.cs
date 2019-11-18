using System;

namespace Cloudents.Command.StudyRooms
{
    public class CreateStudyRoomSessionCommand : ICommand
    {
        public CreateStudyRoomSessionCommand(Guid studyRoomId,
            bool recordVideo,
            long userId, Uri callbackUrl)
        {
            StudyRoomId = studyRoomId;
            RecordVideo = recordVideo;
            UserId = userId;
            CallbackUrl = callbackUrl;
        }
        public Guid StudyRoomId { get; }

        public bool RecordVideo { get; }

        public long UserId { get; }
        public Uri CallbackUrl { get; }
    }
}
