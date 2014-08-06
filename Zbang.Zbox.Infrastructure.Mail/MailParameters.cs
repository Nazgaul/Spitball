using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public abstract class MailParameters
    {
        internal const string WelcomeResolver = "Welcome";
        internal const string InvitationToCloudentsResolver = "InviteCloudents";
        internal const string ForgotPswResolver = "Forgot";
        internal const string InviteResolver = "Invite";
        internal const string MessageResolver = "Message";
        internal const string UpdateResolver = "Update";
        internal const string ChangeEmailResolver = "ChangeEmail";
        internal const string FlagBadItemResolver = "FlagBadItem";
        internal const string PartnersResolver = "Partners";
        internal const string OrderResolver = "Order";

        internal const string DefaultEmail = "no-reply@cloudents.com";

        private const string DefaultSenderName = "Cloudents";

        protected MailParameters(CultureInfo culture, string senderEmail = DefaultEmail, string senderName = DefaultSenderName)
        {
            UserCulture = culture;
            SenderEmail = senderEmail;
            SenderName = senderName;
        }

        public abstract string MailResover { get; }
        public CultureInfo UserCulture { get; private set; }

        public string SenderEmail { get; private set; }
        public string SenderName { get; private set; }
    }
}
