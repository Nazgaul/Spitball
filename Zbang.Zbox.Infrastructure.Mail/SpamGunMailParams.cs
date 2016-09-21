using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{

    public class SpamGunMailParams : MailParameters
    {
        public SpamGunMailParams(
             string body,
            string universityUrl, string name, string subject, string category) 
            :base(new System.Globalization.CultureInfo("en-US"),senderName: "Michael Baker <michael@spitball.co>")
        {
            Body = body;
            UniversityUrl = universityUrl;
            Name = name;
            Subject = subject;
            Category = category;
        }

        public string Body { get; private set; }
        public string Name { get; private set; }
        public string UniversityUrl { get; private set; }

        public string Subject { get; private set; }

        public string Category { get; private set; }

        public override string MailResover => nameof(SpamGunMailParams);
    }
}
