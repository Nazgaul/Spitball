using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class ReplyToCommentMail : IMailBuilder
    {
        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            var mailParams = parameters as ReplyToCommentMailParams;
            if (mailParams == null)
            {
                throw new NullReferenceException(nameof(mailParams));
            }


            message.SetCategory("Reply to comment");
            var html = LoadMailTempate.LoadMailFromContentWithDot(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.MarketingTemplate");
            html.Replace("{name}", mailParams.UserName);

            var link = $"<a href=\"{UrlConsts.AppendCloudentsUrl(mailParams.BoxUrl)}\">{EmailResource.ReplyToCommentLink}</a>";
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(string.Format(EmailResource.ReplyToCommentText, mailParams.UserWhoMadeActionName,mailParams.BoxName, link));
            html.Replace("{body}", sb.ToString().Replace("\n", "<br>"));
            message.Html = html.ToString();
        }

        public void AddSubject(ISendGrid message)
        {
            message.Subject = EmailResource.ReplyToCommentSubject;
        }
    }
}
