using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using env = System.Environment;

namespace Cloudents.Core.Message.Email
{
    public class RequestTutorEmail : BaseEmail
    {
        

        public RequestTutorEmail()
            : base("support@spitball.co", null, null)
        {
            Dictionary = new Dictionary<string, string>();
        }

        public Dictionary<string,string> Dictionary { get; set; }

        //public long? UserId { get;  set; }

        //public string Text { get;  set; }

        //public string Course { get;  set; }

        public string Email { get;  set; }
        //public string Name { get;  set; }
        //public string University { get;  set; }
        //public string Country { get;  set; }

        //public string PhoneNumber { get;  set; }

        public bool IsProduction { get; set; }

        //public string Referer { get; set; }

        public override string Subject => $"Request Tutor anonymous Email {Email} IsProduction {IsProduction}";

        public override string[] Bcc => new[] { "eidan@cloudents.com", "jaron@spitball.co", "ram@cloudents.com", "elad@cloudents.com" };

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var entry in Dictionary)
            {
                sb.AppendLine($"{entry.Key}: {entry.Value}");
            }
            //if (UserId.HasValue)
            //{
            //    sb.AppendLine($"{nameof(UserId)}: {UserId.Value}");
            //}

            //sb.AppendLine($"UserName: {Name}");
            //sb.AppendLine($"Email: {Email}");
            //sb.AppendLine($"University: {University}");
            //sb.AppendLine($"Country: {Country}");
            //sb.AppendLine($"Phone number: {PhoneNumber}");
            //sb.AppendLine($"Text: {Text}");
            //sb.AppendLine($"Course: {Course}");
            //sb.AppendLine($"Referer: {Referer}");
           
          

            sb.Replace(env.NewLine, "<br>");
            return sb.ToString();
        }

        public override string Campaign => "RequestTutor";
        protected override IDictionary<CultureInfo, string> Templates => null;
    }
}