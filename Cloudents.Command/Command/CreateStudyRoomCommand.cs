using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Command.Command
{
    public class CreateStudyRoomCommand : ICommand
    {
        public CreateStudyRoomCommand(long tutorId, IEnumerable<long>? studentsId, string textMessage, string name, decimal price)
        {
            TutorId = tutorId;
            StudentsId = studentsId ?? Enumerable.Empty<long>();
            TextMessage = textMessage;
            Name = name;
        }

        public long TutorId { get; }
        public IEnumerable<long> StudentsId { get; }

        public string TextMessage { get;  }

        public string Name { get; }

        public decimal Price { get; set; }
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