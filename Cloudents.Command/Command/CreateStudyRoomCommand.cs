using System;

namespace Cloudents.Command.Command
{
    public class CreateStudyRoomCommand : ICommand
    {
        public CreateStudyRoomCommand(long tutorId, long studentId, string textMessage)
        {
            TutorId = tutorId;
            StudentId = studentId;
            TextMessage = textMessage;
        }

        public long TutorId { get; }
        public long StudentId { get; }

        public string TextMessage { get;  }
    }

    public class CreateStudyRoomCommandResult : ICommandResult
    {
        public CreateStudyRoomCommandResult( Guid studyRoomId)
        {
            StudyRoomId = studyRoomId;
        }

        public Guid StudyRoomId { get; set; }

    }
}