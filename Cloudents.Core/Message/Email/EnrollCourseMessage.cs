using System.Collections.Generic;
using System.Globalization;

namespace Cloudents.Core.Message.Email
{
    public class EnrollCourseMessage : BaseEmail
    {
        public EnrollCourseMessage(string to, string tutorFirstName,string userFirstName, string courseName, string userEmail)
        :base("ram@cloudents.com","Great news someone enrolled to you course Spitball",null)
        {
            Body = @$"Hey {tutorFirstName}
You have a new student on your service on Spitball. {userFirstName} has enrolled for {courseName}.

If you wish to send them a personal welcome email here is email : {userEmail}.

The Spitball Team";
        }

        public override string? Campaign => null;
        public override UnsubscribeGroup UnsubscribeGroup => UnsubscribeGroup.Update;
        protected override IDictionary<CultureInfo, string>? Templates => null;
        public string Body { get; private set; }

        public override string ToString()
        {
            return Body;
        }
    }
}