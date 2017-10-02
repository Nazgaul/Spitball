using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class NoUniversityMailParams : MarketingMailParams
    {
        public NoUniversityMailParams(string name, CultureInfo culture)
            : base(name, culture)
        {
        }

        public override string MailResolver => nameof(NoUniversityMailParams);
    }
}
