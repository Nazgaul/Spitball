using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class WelcomeMail : MailBuilder
    {
        private const string Category = "Welcome";

        private readonly WelcomeMailParams m_Parameters;

        public WelcomeMail(MailParameters parameters) : base(parameters)
        {
            m_Parameters = parameters as WelcomeMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(m_Parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.Welcome");
            return html.Replace("{Name}", m_Parameters.Name);
        }

        public override string AddSubject()
        {
            return EmailResource.WelcomeSubject;
        }

        public override string AddCategory()
        {
            return Category;
        }
    }
}
