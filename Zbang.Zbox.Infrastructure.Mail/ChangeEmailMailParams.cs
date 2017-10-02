using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class ChangeEmailMailParams : MailParameters
    {
        public ChangeEmailMailParams(string code, CultureInfo culture)
            : base(culture)
        {
            Code = code;
        }

        public string Code { get; }

        public override string MailResolver => ChangeEmailResolver;
    }
}