namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class InvitationToCloudentsMail : MailBuilder
    {
        private const string Category = "InvitationCloudents";
        private const string Subject = "Invite to Spitball";

        private readonly InvitationToCloudentsMailParams m_Parameters;

        public InvitationToCloudentsMail(MailParameters parameters) : base(parameters)
        {
            m_Parameters = parameters as InvitationToCloudentsMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(m_Parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.InviteCloudents");

            html = html.Replace("{USERNAME}", m_Parameters.SenderName);
            html = html.Replace("{Image}", m_Parameters.SenderImage);
            html = html.Replace("{{Url}}", m_Parameters.Url);
            return html;
        }

        public override string AddSubject()
        {
            return Subject;
        }

        public override string AddCategory()
        {
            return Category;
        }
    }
}
