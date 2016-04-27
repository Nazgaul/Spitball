using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class NoFollowingBoxMailParams : MarketingMailParams
    {
        public NoFollowingBoxMailParams(string name, CultureInfo culture)
            : base(name, culture)
        {
        }

        public override string MailResover => nameof(NoFollowingBoxMailParams);
    }

    public class LowContributionMailParams : MarketingMailParams
    {
        public LowContributionMailParams(string name, CultureInfo culture)
            : base(name, culture)
        {
        }

        public override string MailResover => nameof(NoFollowingBoxMailParams);
    }
}