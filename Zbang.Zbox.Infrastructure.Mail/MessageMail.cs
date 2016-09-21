using System.Text;
using SendGrid;
using System;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class MessageMail : MailBuilder
    {
        private const string Category = "Personal Message";
        private const string Subject = "Message via Spitball from {0}";

        private readonly MessageMailParams m_Parameters;

        public MessageMail(MailParameters parameters) : base(parameters)
        {
            m_Parameters = parameters as MessageMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(m_Parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.PersonalMsg");
            html = html.Replace("{USER_MSG}", m_Parameters.Message);
            html = html.Replace("{MSG_AUTHOR}", m_Parameters.SenderName);
            html = html.Replace("{ImgUrl}", m_Parameters.SenderImage);
            //sb.Replace("{REPLY_URL}", messageParams.SenderEmail);

            return html;


        }

        public override string AddCategory()
        {
            return Category;
        }

        public override string AddSubject()
        {
            return string.Format(Subject, m_Parameters.SenderName);
        }
    }
}
