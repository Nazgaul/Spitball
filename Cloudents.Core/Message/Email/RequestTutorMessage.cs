using System;
using System.Collections.Generic;
using System.Globalization;
using Cloudents.Core.Message.System;

namespace Cloudents.Core.Message.Email
{
    public class RequestTutorMessage : ISystemQueueMessage
    {
        public RequestTutorMessage(Guid leadId)
        {
            LeadId = leadId;
        }

        public Guid LeadId{ get; private set; }
    }

    public class EnrollCourseMessage : BaseEmail
    {
        public EnrollCourseMessage(string tutorFirstName,string userFirstName, string courseName, string userEmail)
        {
            Subject = "Great news some one enrolled to you course Spitball";
            Body = @$"Hey {tutorFirstName}
You have a new student on your service on Spitball. {userFirstName} has enrolled for {courseName}.

If you wish to send them a personal welcome email here is email : {userEmail}.

The Spitball Team";
        }

        public override string? Campaign => null;
        public override UnsubscribeGroup UnsubscribeGroup => UnsubscribeGroup.Update;
        protected override IDictionary<CultureInfo, string>? Templates => null;
        public string Body { get; set; }

        public override string ToString()
        {
            return Body;
        }
    }
}