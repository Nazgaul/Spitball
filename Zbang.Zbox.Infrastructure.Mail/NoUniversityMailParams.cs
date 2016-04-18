using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class NoUniversityMailParams : MailParameters
    {
        public NoUniversityMailParams(string name, CultureInfo culture)
            : base(culture)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public override string MailResover => nameof(NoUniversityMailParams);

    }
}
