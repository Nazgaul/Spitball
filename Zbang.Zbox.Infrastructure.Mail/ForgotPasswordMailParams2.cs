using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class ForgotPasswordMailParams2 : MailParameters
    {
        public ForgotPasswordMailParams2(string code, string link, string name, CultureInfo culture)
            : base(culture)
        {
            Code = code;
            Link = link;
            Name = name;
        }

        public string Code { get; }
        public string Link { get; }
        public string Name { get; }
        public override string MailResolver => ForgotPswResolver;
    }
}