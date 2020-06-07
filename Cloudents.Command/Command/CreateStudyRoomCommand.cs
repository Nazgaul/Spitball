﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Command.Command
{
    public class CreateStudyRoomCommand : ICommand
    {
        public CreateStudyRoomCommand(long tutorId, IEnumerable<long>? studentsId, string textMessage,
            string name, decimal price, DateTime? broadcastTime, StudyRoomType type, string? description)
        {
            TutorId = tutorId;
            StudentsId = studentsId ?? Enumerable.Empty<long>();
            TextMessage = textMessage;
            Name = name;
            Price = price;
            BroadcastTime = broadcastTime;
            Type = type;
            Description = description;
        }

        public long TutorId { get; }
        public IEnumerable<long> StudentsId { get; }

        public string TextMessage { get;  }

        public string Name { get; }

        public decimal Price { get;  }
        public DateTime? BroadcastTime { get; }

        public StudyRoomType Type { get; }
        public string? Description { get; }

        public Guid StudyRoomId { get; set; }
        public string Identifier { get; set; }
    }

   
}