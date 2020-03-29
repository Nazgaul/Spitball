using System;
using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class CreateStudyRoomCommand : ICommand
    {
        public CreateStudyRoomCommand(long tutorId, IEnumerable<long> studentsId, string textMessage)
        {
            TutorId = tutorId;
            StudentsId = studentsId;
            TextMessage = textMessage;
        }

        public long TutorId { get; }
        public IEnumerable<long> StudentsId { get; }

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