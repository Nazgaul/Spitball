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


    public class GreekPartnerMailParams : MailParameters
    {
        public GreekPartnerMailParams(
             string body,
            string universityUrl, string name, string subject, string category, string school, string chapter)
            : base(new System.Globalization.CultureInfo("en-US"), senderName: "Justin Liao <justin@spitball.co>")
        {
            Body = body;
            UniversityUrl = universityUrl;
            Name = name;
            Subject = subject;
            Category = category;
            School = school;
            Chapter = chapter;
        }

        public string Body { get; private set; }
        public string Name { get; private set; }
        public string UniversityUrl { get; private set; }

        public string Subject { get; private set; }

        public string School { get; private set; }
        public string Chapter { get; private set; }

        public string Category { get; private set; }
        public override string MailResover => nameof(GreekPartnerMailParams);
    }
}
