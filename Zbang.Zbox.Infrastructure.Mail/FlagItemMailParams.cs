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
        public override string MailResover
        {
            get { return FlagBadItemResolver; }
        }
        public string ItemName { get; private set; }
        public string Reason { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }

        public string Url { get; private set; }
    }
}