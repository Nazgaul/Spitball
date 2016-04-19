using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class NoUniversityMailParams : MarketingMailParams
    {
        public NoUniversityMailParams(string name, CultureInfo culture)
            : base(name, culture)
        {
        }

        public override string MailResover => nameof(NoUniversityMailParams);

    }

    public class NoFollowingBoxMailParams : MarketingMailParams
    {
        public NoFollowingBoxMailParams(string name, CultureInfo culture)
            : base(name, culture)
        {
        }

        public override string MailResover => nameof(NoFollowingBoxMailParams);
    }

    public abstract class MarketingMailParams : MailParameters
    {
        protected MarketingMailParams(string name, CultureInfo culture)
            : base(culture)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public abstract override string MailResover { get; }
    }
}
