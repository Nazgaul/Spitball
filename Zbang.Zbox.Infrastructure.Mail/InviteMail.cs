using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class InviteMail : MailBuilder
    {
        private const string Category = "Invite";

        private readonly InviteMailParams m_Parameters;

        public InviteMail(MailParameters parameters) : base(parameters)
        {
            m_Parameters = parameters as InviteMailParams;
        }

        public override string GenerateMail()
        {

            var html = LoadMailTempate.LoadMailFromContent(m_Parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.InviteCourse");
            html = html.Replace("{INVITOR}", m_Parameters.Invitor);
            html = html.Replace("{BOXNAME}", m_Parameters.BoxName);
            html = html.Replace("{BoxUrl}", m_Parameters.BoxUrl);
            html = html.Replace("{ImgUrl}", m_Parameters.InvitorImage);
            return html;
        }

        public override string AddSubject()
        {
            return string.Format(EmailResource.InviteSubject, m_Parameters.BoxName);
        }

        public override string AddCategory()
        {
            return Category;
        }
    }
}
