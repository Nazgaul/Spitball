using System.Globalization;
using System.Text;

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
            //var html = LoadMailTempate.LoadMailFromContentWithDot(new CultureInfo("en-US"), "Zbang.Zbox.Infrastructure.Mail.MailTemplate.SpamGun");
            var html = new StringBuilder(m_Parameters.HtmlBody);
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

    public class GreekPartnerMail : MailBuilder
    {
        private readonly GreekPartnerMailParams m_Parameters;
        public GreekPartnerMail(MailParameters parameters) : base(parameters)
        {
            m_Parameters = parameters as GreekPartnerMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContentWithDot(new CultureInfo("en-US"), "Zbang.Zbox.Infrastructure.Mail.MailTemplate.GreekHouseTemplate");
            html.Replace("{name}", m_Parameters.Name);
            html.Replace("{body}", m_Parameters.Body.Replace("\n", "<br>"));
            html.Replace("{uni_Url}", m_Parameters.UniversityUrl);
            html.Replace("{School}", m_Parameters.School);
            html.Replace("{Chapter}", m_Parameters.Chapter);
            return html.ToString();
        }

        public override string AddSubject()
        {
            return m_Parameters.Subject.Replace("{Chapter}", m_Parameters.Chapter);
        }

        public override string AddCategory()
        {
            return m_Parameters.Category;
        }
    }
}