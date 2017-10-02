using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class UniversityLowActivityMailParams : MarketingMailParams
    {
        public UniversityLowActivityMailParams(string name, CultureInfo culture)
            : base(name, culture)
        {
        }

        public override string MailResolver => nameof(UniversityLowActivityMailParams);
    }
}