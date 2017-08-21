using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public abstract class MailParameters
    {
        internal const string WelcomeResolver = "Welcome";
        internal const string DepartmentRequestAccessResolver = "Department";
        internal const string DepartmentRequestApprovedResolver = "DepartmentApproved";
        internal const string InvitationToCloudentsResolver = "InviteCloudents";
        internal const string ForgotPswResolver = "Forgot";
        internal const string InviteResolver = "Invite";
        internal const string MessageResolver = "Message";
        internal const string UpdateResolver = "Update";
        internal const string ChangeEmailResolver = "ChangeEmail";
        internal const string FlagBadItemResolver = "FlagBadItem";

        public const string DefaultEmail = "no-reply@Spitball.co";

        private const string DefaultSenderName = "Spitball";

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
