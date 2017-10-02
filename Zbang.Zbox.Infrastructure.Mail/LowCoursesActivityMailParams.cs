using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class LowCoursesActivityMailParams : MarketingMailParams
    {
        public LowCoursesActivityMailParams(string name, CultureInfo culture)
            : base(name, culture)
        {
        }

        public override string MailResolver => nameof(LowCoursesActivityMailParams);
    }
}