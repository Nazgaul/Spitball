namespace Zbang.Zbox.Infrastructure.Mail
{
    public class SpamGunMailParams : MailParameters
    {
        public SpamGunMailParams(
             string body,
            string universityUrl, string name, string subject, string category)
            :base(new System.Globalization.CultureInfo("en-US"),senderName: "Olivia Williams <olivia@spitball.co>")
        {
            Body = body;
            UniversityUrl = universityUrl;
            Name = name;
            Subject = subject;
            Category = category;
        }

        public string Body { get; }
        public string Name { get; }
        public string UniversityUrl { get; }

        public string Subject { get; }

        public string Category { get; }
        public override string MailResolver => nameof(SpamGunMailParams);
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

        public string Body { get; }
        public string Name { get; }
        public string UniversityUrl { get; }

        public string Subject { get; }

        public string School { get; }
        public string Chapter { get; }

        public string Category { get; }
        public override string MailResolver => nameof(GreekPartnerMailParams);
    }
}
