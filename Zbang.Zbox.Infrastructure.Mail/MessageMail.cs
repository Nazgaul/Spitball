using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class MessageMail : IMailBuilder
    {
        const string Category = "Personal Message";
        const string Subject = "Message via Cloudents from {0}";

        //"Message via Cloudents from " + parameters.FromUserName   {USER_MSG}

        public void GenerateMail(SendGridMail.ISendGrid message, MailParameters parameters)
        {
            var messageParams = parameters as MessageMailParams;
            Zbang.Zbox.Infrastructure.Exceptions.Throw.OnNull(messageParams, "messageParams");

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.PersonalMsg");
            //message.Text = textBody;
            message.Subject = string.Format(Subject, messageParams.SenderName);
            var sb = new StringBuilder(message.Html);
            sb.Replace("{USER_MSG}", messageParams.Message);
            sb.Replace("{MSG_AUTHOR}", messageParams.SenderName);
            sb.Replace("{ImgUrl}", messageParams.SenderImage);
            sb.Replace("{REPLY_URL}", messageParams.SenderEmail);

            message.Html = sb.ToString();
            //message.Html = message.Html.Replace("{USER_MSG}", messageParams.Message);
            //message.Html = message.Html.Replace("{MSG_AUTHOR}", messageParams.SenderName);
            //message.Html = message.Html.Replace("{ImgUrl}", messageParams.SenderImage);
            //message.Html = message.Html.Replace("{REPLY_URL}", messageParams.SenderEmail);
            //{MSG_AUTHOR}
            //{ImgUrl}
            
        }
    }
}
