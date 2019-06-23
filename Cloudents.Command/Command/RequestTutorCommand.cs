using System;

namespace Cloudents.Command.Command
{
    public class RequestTutorCommand : ICommand
    {
        public RequestTutorCommand(string course,
            string chatText,
            long userId,
            Guid? universityId,
            string referer,
            string name,
            string phoneNumber,
            string leadText,
            string email, long? tutorId, string utmSource)
        {
            Course = course;
            ChatText = chatText;
            if (userId > 0)
            {
                UserId = userId;
            }

            UniversityId = universityId;
            Referer = referer;
            Name = name;
            PhoneNumber = phoneNumber;
            LeadText = leadText;
            Email = email;
            TutorId = tutorId;
            UtmSource = utmSource;
        }

        public string Course { get;  }
        public long? UserId { get; }
        public string ChatText { get; }
        public Guid? UniversityId { get;  }
        public string Referer { get;  }
        public string Name { get;  }
        public string PhoneNumber { get;}
        public string LeadText { get; }
        public string Email { get; }
        public long? TutorId { get; }
        public string UtmSource { get; }
    }
}