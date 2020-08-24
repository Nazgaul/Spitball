using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Command.Command
{
    public class CreatePrivateStudyRoomCommand : ICommand
    {
        public CreatePrivateStudyRoomCommand(long tutorId, IEnumerable<long>? studentsId, string textMessage,
            string name, decimal price)
        {
            TutorId = tutorId;
            StudentsId = studentsId ?? Enumerable.Empty<long>();
            TextMessage = textMessage;
            Name = name;
            Price = price;
        }

        public long TutorId { get; }
        public IEnumerable<long> StudentsId { get; }

        public string TextMessage { get;  }

        public string Name { get; }

        public decimal Price { get;  }

        public Guid StudyRoomId { get; set; }
        public string Identifier { get; set; }
    }
}