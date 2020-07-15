using System;
using System.Collections.Generic;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class UserStudyRoomDto
    {
        public UserStudyRoomDto(string name, Guid id, DateTime dateTime,
            string conversationId, DateTime? lastSession, StudyRoomType type,
            DateTime? scheduled, IEnumerable<string> userNames, Money money,long tutorId, string tutorName)
        {
            Name = name;
            Id = id;
            DateTime = dateTime;
            ConversationId = conversationId;
            LastSession = lastSession;
            Type = type;
            Scheduled = scheduled;
            UserNames = userNames;
            Price = money;
            TutorId = tutorId;
            TutorName = tutorName;

        }

        public UserStudyRoomDto()
        {
        }

        public string Name { get; set; }
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }

        public string ConversationId { get; set; }
        public DateTime? LastSession { get; set; }

        public StudyRoomType Type { get; set; }

        public DateTime? Scheduled { get; set; }

        public IEnumerable<string> UserNames { get; set; }
        public Money Price { get; }

        public long TutorId { get; set; }
        public string TutorName { get; set; }

    }
}