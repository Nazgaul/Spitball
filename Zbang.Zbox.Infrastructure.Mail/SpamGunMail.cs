using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class SpamGunMail : MailBuilder
    {
        private readonly SpamGunMailParams m_Parameters;
        public SpamGunMail(MailParameters parameters) : base(parameters)
        {
            m_Parameters = parameters as SpamGunMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContentWithDot(new CultureInfo("en-US"), "Zbang.Zbox.Infrastructure.Mail.MailTemplate.SpamGun");
            html.Replace("{name}", m_Parameters.Name);
            html.Replace("{body}", m_Parameters.Body.Replace("\n", "<br>"));
            html.Replace("{uni_Url}", m_Parameters.UniversityUrl);
            return html.ToString();
        }

        public override string AddSubject()
        {
            return m_Parameters.Subject;
        }

        public override string AddCategory()
        {
            return m_Parameters.Category;
        }
    }
}