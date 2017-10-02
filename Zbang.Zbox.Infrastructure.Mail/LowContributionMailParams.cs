using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class LowContributionMailParams : MarketingMailParams
    {
        public LowContributionMailParams(string name, CultureInfo culture)
            : base(name, culture)
        {
        }

        public override string MailResolver => nameof(NoFollowingBoxMailParams);
    }
}