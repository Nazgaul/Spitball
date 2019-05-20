using Cloudents.Core.DTOs;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using env = System.Environment;

namespace Cloudents.Core.Message.Email
{
    public class RequestTutorEmail : BaseEmail
    {
        //public RequestTutorEmail(long userId, string text, string course,
        //    UserEmailInfoDto info,  string[] links) :
        //    base("support@spitball.co", $"Request Tutor Email {userId}", null)
        //{
        //    UserId = userId;
        //    Text = text;
        //    Course = course;
        //    Links = links ?? new string[0];
        //    Email = info.Email;
        //    Name = info.Name;
        //    University = info.University;
        //    Country = info.Country;
        //    PhoneNumber = info.PhoneNumber;
        //    Bcc = new[] { "eidan@cloudents.com", "jaron@spitball.co", "ram@cloudents.com", "elad@cloudents.com" };
        //}

        //TODO: this is nor optimal but it working.
        public RequestTutorEmail(long userId, string text, string course,
           string email, string name, string university, string country, string phoneNumber, string[] links, bool isProduction) :
           base("support@spitball.co", $"Request Tutor Email {userId} IsProduction {isProduction}", null)
        {
            UserId = userId;
            Text = text;
            Course = course;
            Links = links ?? new string[0];
            Email = email;
            Name = name;
            University = university;
            Country = country;
            PhoneNumber = phoneNumber;
            Bcc = new[] { "eidan@cloudents.com", "jaron@spitball.co", "ram@cloudents.com", "elad@cloudents.com" };
        }

        public RequestTutorEmail( string text, string course,
            string email, string name, string university, string country, string phoneNumber, string[] links, bool isProduction) :
            base("support@spitball.co", $"Request Tutor anonymous Email {email} IsProduction {isProduction}", null)
        {
            Text = text;
            Course = course;
            Links = links ?? new string[0];
            Email = email;
            Name = name;
            University = university;
            Country = country;
            PhoneNumber = phoneNumber;
            Bcc = new[] { "eidan@cloudents.com", "jaron@spitball.co", "ram@cloudents.com", "elad@cloudents.com" };
        }

        public RequestTutorEmail()
        {
            
        }


        public long? UserId { get; private set; }

        public string Text { get; private set; }

        public string Course { get; private set; }

        public string[] Links { get; private set; }
        public string Email { get; private set; }
        public string Name { get; private set; }
        public string University { get; private set; }
        public string Country { get; private set; }

        public string PhoneNumber { get; private set; }


        public override string ToString()
        {
            var sb = new StringBuilder();
            if (UserId.HasValue)
            {
                sb.AppendLine($"{nameof(UserId)}: {UserId.Value}");
            }

            sb.AppendLine($"UserName: {Name}");
            sb.AppendLine($"Email: {Email}");
            sb.AppendLine($"University: {University}");
            sb.AppendLine($"Country: {Country}");
            sb.AppendLine($"Phone number: {PhoneNumber}");
            sb.AppendLine($"Text: {Text}");
            sb.AppendLine($"Course: {Course}");
           
            if (Links != null && Links.Length > 0)
            {
               // sb.Append("<BR>");
                sb.AppendLine($" {nameof(Links)}: {string.Join("<br>", Links.Select((s, i) => $"<a href='{s}'> attachment {++i}</a>"))}");
            }

            sb.Replace(env.NewLine, "<br>");
            return sb.ToString();
        }

        public override string Campaign => "RequestTutor";
        protected override IDictionary<CultureInfo, string> Templates => null;
    }
}