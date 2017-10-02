using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public abstract class MarketingMailParams : MailParameters
    {
        protected MarketingMailParams(string name, CultureInfo culture)
            : base(culture)
        {
            Name = name;
        }

        public string Name { get; }

        public abstract override string MailResolver { get; }
    }
}