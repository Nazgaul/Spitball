using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cloudents.Core.Message.Email
{
    public class RequestTutorEmail : BaseEmail
    {
        public RequestTutorEmail(long userId, string text, string course, string[] links) :
            base("support@spitball.co", $"Request Tutor Email", null)
        {
            UserId = userId;
            Text = text;
            Course = course;
            Links = links ?? new string[0];
            Bcc = new[] { "eidan@cloudents.com", "jaron@spitball.co", "ram@cloudents.com" };
        }

        public long UserId { get; private set; }

        public string Text { get; private set; }

        public string Course { get; private set; }

        public string[] Links { get; private set; }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{nameof(UserId)}: {UserId}<br> {nameof(Text)}: {Text}<br> {nameof(Course)}: {Course},");
            if (Links != null && Links.Length > 0)
            {
                sb.Append("<BR>");
                sb.Append($" {nameof(Links)}: {string.Join("<br>", Links.Select((s, i) => $"<a href='{s}'> attachment {i}</a>"))}");
            }

            return sb.ToString();
        }

        public override string Campaign => "RequestTutor";
        protected override IDictionary<CultureInfo, string> Templates => null;
    }
}