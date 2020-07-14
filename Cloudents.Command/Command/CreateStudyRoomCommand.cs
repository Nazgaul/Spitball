using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Enum;

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
    public class CreateLiveStudyRoomCommand : ICommand
    {
        public CreateLiveStudyRoomCommand(long tutorId, 
            string name, decimal price, DateTime? broadcastTime,  string? description)
        {
            TutorId = tutorId;
            Name = name;
            Price = price;
            BroadcastTime = broadcastTime;
            Description = description;
        }

        public long TutorId { get; }


        public string Name { get; }

        public decimal Price { get;  }
        public DateTime? BroadcastTime { get; }

        public string? Description { get; }

        public Guid StudyRoomId { get; set; }
        public string Identifier { get; set; }
    }

   
}