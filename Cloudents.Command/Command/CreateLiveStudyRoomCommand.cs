//using System;
//using System.Collections.Generic;
//using Cloudents.Core.Enum;

//namespace Cloudents.Command.Command
//{
//    public class CreateLiveStudyRoomCommand : ICommand
//    {
//        public CreateLiveStudyRoomCommand(long tutorId, 
//            string name, decimal price, DateTime broadcastTime,  string description, StudyRoomRepeat? repeat, DateTime? endDate, int? endAfterOccurrences, IEnumerable<DayOfWeek> repeatOn, string? image)
//        {
//            TutorId = tutorId;
//            Name = name;
//            Price = price;
//            BroadcastTime = broadcastTime;
//            Description = description;
//            Repeat = repeat;
//            EndDate = endDate;
//            EndAfterOccurrences = endAfterOccurrences;
//            RepeatOn = repeatOn;
//            Image = image;
//        }

//        public long TutorId { get; }


//        public string Name { get; }

//        public decimal Price { get;  }
//        public DateTime BroadcastTime { get; }

//        public string Description { get; }


//        public StudyRoomRepeat? Repeat { get; }

//        public DateTime? EndDate { get; }
//        public int? EndAfterOccurrences { get;  }

//        public IEnumerable<DayOfWeek> RepeatOn { get;  }


//        public Guid StudyRoomId { get; set; }
//        public string? Identifier { get; set; }

//        public string? Image { get; }


//    }
//}