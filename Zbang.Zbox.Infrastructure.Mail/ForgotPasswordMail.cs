namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class ForgotPasswordMail : MailBuilder
    {
        private const string Category = "Password Recovery";
        private const string Subject = "Password Recovery";

        private readonly ForgotPasswordMailParams2 m_Parameters;

        public ForgotPasswordMail(MailParameters parameters) : base(parameters)
        {
            m_Parameters = parameters as ForgotPasswordMailParams2;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(m_Parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.ResetPwd");
            html = html.Replace("{NEW-PWD}", m_Parameters.Code);
            html = html.Replace("{CHANGE-URL}", m_Parameters.Link);
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
