using System;
using System.Collections.Generic;
using System.Globalization;

namespace Cloudents.Core.Message.Email
{
    public class EnrollCourseMessage : BaseEmail
    {
        public string TutorFirstName { get; private set; }
        public string UserFirstName { get;private set; }
        public string CourseName { get; private set;}
        public string UserEmail { get;private set; }

        public EnrollCourseMessage(string to, string tutorFirstName,string userFirstName,
            string courseName, string userEmail)
        :base("ram@cloudents.com","Great news someone enrolled to you course Spitball",null)
        {
            TutorFirstName = tutorFirstName;
            UserFirstName = userFirstName;
            CourseName = courseName;
            UserEmail = userEmail;
        }

        

        //protected EnrollCourseMessage() : base()
        //{

        //}

        public override string? Campaign => null;
        public override UnsubscribeGroup UnsubscribeGroup => UnsubscribeGroup.Update;
        protected override IDictionary<CultureInfo, string>? Templates => null;

        public override string ToString()
        {
            return @$"Hey {TutorFirstName}
                You have a new student on your service on Spitball. {UserFirstName} has enrolled for {CourseName}.

                If you wish to send them a personal welcome email here is email : {UserEmail}.

                The Spitball Team".Replace(Environment.NewLine,"<br>");
        }
    }
}