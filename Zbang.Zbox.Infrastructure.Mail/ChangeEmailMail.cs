namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class ChangeEmailMail : MailBuilder
    {
        private const string Category = "Change Email";
        private const string Subject = "Change Email";

        private readonly ChangeEmailMailParams m_Parameters;

        public ChangeEmailMail(MailParameters parameters) : base(parameters)
        {
            m_Parameters = parameters as ChangeEmailMailParams;
        }
        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(m_Parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.ChangeEmail");
            return html.Replace("{CODE}", m_Parameters.Code);
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
