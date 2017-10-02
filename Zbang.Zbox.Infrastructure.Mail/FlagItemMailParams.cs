using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class FlagItemMailParams : MailParameters
    {
        public FlagItemMailParams(string itemName, string reason, string userName, string email, string url)
            : base(new CultureInfo("en-Us"))
        {
            ItemName = itemName;
            Reason = reason;
            UserName = userName;
            Email = email;
            Url = url;
        }

        public override string MailResolver => FlagBadItemResolver;
        public string ItemName { get; }
        public string Reason { get; }
        public string UserName { get; }
        public string Email { get; }

        public string Url { get; }
    }
}