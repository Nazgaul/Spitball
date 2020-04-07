﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Command.Command
{
    public class CreateStudyRoomCommand : ICommand
    {
        public CreateStudyRoomCommand(long tutorId, IEnumerable<long>? studentsId, string textMessage,
            string name, decimal price, DateTime? broadcastTime, StudyRoomType type)
        {
            TutorId = tutorId;
            StudentsId = studentsId ?? Enumerable.Empty<long>();
            TextMessage = textMessage;
            Name = name;
            Price = price;
            BroadcastTime = broadcastTime;
            Type = type;
        }

        public long TutorId { get; }
        public IEnumerable<long> StudentsId { get; }

        public string TextMessage { get;  }

        public string Name { get; }

        public decimal Price { get;  }
        public DateTime? BroadcastTime { get; }

        public StudyRoomType Type { get; }
    }

    public class CreateStudyRoomCommandResult : ICommandResult
    {
        public CreateStudyRoomCommandResult( Guid studyRoomId, string identifier)
        {
            StudyRoomId = studyRoomId;
            Identifier = identifier;
        }

        public Guid StudyRoomId { get;  }

        public string Identifier { get; }

    }
}