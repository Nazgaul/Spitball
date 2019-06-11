using System;

namespace Cloudents.Core.Message
{
    public class MondayMessage
    {
        public MondayMessage(string course, bool isProduction, string name, string phoneNumber, string text, string university, string utmSource)
        {
            Course = course;
            IsProduction = isProduction;
            Name = name;
            PhoneNumber = phoneNumber;
            Text = text;
            University = university;
            UtmSource = utmSource;
        }

        public bool IsProduction { get; }
        public string Name { get;  }
        public string UtmSource { get;  }
        public string PhoneNumber { get;  }
        public string Text { get; }
        public string Course { get; }
        public string University { get;  }
    }
}