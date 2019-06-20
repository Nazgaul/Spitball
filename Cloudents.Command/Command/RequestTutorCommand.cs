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
            string email)
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
    }
}