using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class WelcomeMailParams : MailParameters
    {
        public WelcomeMailParams(string name, CultureInfo culture)
            : base(culture)
        {
            Name = name;
        }
        public string Name { get; private set; }

        public override string MailResover
        {
            get { return WelcomeResolver; }
        }
    }
}