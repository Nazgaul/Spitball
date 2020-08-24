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

        public string Name { get;  }
        public Guid Id { get;  }
        public DateTime DateTime { get;  }

        public string ConversationId { get;  }
        public DateTime? LastSession { get;  }

        public StudyRoomType Type { get;  }

        public DateTime? Scheduled { get;  }

        public IEnumerable<string> UserNames { get;  }
        public Money Price { get; }

        public long TutorId { get;  }
        public string TutorName { get;  }

    }
}