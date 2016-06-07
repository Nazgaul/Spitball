using System.Text;
using SendGrid;
using System;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class MessageMail : IMailBuilder
    {
        const string Category = "Personal Message";
        const string Subject = "Message via Spitball from {0}";

        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            var messageParams = parameters as MessageMailParams;
            if (messageParams == null)
            {
                throw new NullReferenceException("messageParams");
            }

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.PersonalMsg");
            message.Subject = string.Format(Subject, messageParams.SenderName);
            var sb = new StringBuilder(message.Html);
            sb.Replace("{USER_MSG}", messageParams.Message);
            sb.Replace("{MSG_AUTHOR}", messageParams.SenderName);
            sb.Replace("{ImgUrl}", messageParams.SenderImage);
            //sb.Replace("{REPLY_URL}", messageParams.SenderEmail);

            message.Html = sb.ToString();
            
        }

        public void AddSubject(ISendGrid message)
        {
            message.Subject = string.Empty;
        }
    }
}
