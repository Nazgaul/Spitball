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

    //public class UploadStudyRoomVideoCommand : ICommand
    //{
    //    public UploadStudyRoomVideoCommand(Guid studyRoomId, long userId, Stream videoStream)
    //    {
    //        StudyRoomId = studyRoomId;
    //        UserId = userId;
    //        VideoStream = videoStream;
    //    }
    //    public Guid StudyRoomId { get; }


    //    public long UserId { get; }

    //    public Stream VideoStream { get; }
    //}

    //public class StudyRoomVideoReadyCommand : ICommand
    //{
    //    public StudyRoomVideoReadyCommand(string sessionIdentifier, Guid studyRoomId)
    //    {
    //        SessionIdentifier = sessionIdentifier;
    //        StudyRoomId = studyRoomId;
    //    }

    //    public string SessionIdentifier { get; }

    //    public Guid StudyRoomId { get; }
    //}


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