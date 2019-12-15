using System.Globalization;
using Cloudents.Core.Message.System;

namespace Cloudents.Core.DTOs.Email
{
    public class RequestTutorEmailDto 
    {
        public CultureInfo TutorLanguage { get; set; }

        public string Request { get; set; }

        public string StudentPhoneNumber { get; set; }
        public string StudentName { get; set; }

        public string CourseName { get; set; }

        public string TutorFirstName { get; set; }

        public long TutorId { get; set; }

        public long StudentId { get; set; }

       // public string Url { get; set; }

        public string TutorEmail { get; set; }
        public string TutorCountry { get; set; }
    }
}
