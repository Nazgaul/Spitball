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
            var html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.ReplyToCommentMailTemplate");
            message.AddSubstitution("{name}", new List<string> { mailParams.UserName });
            message.AddSubstitution("{UserWhoMadeActionName}", new List<string> { mailParams.UserWhoMadeActionName });
            message.AddSubstitution("{BoxName}", new List<string> { mailParams.BoxName });
            message.AddSubstitution("{BoxUrl}", new List<string> { UrlConst.AppendCloudentsUrl(mailParams.BoxUrl) });
            message.EnableGoogleAnalytics("cloudentsMail", "email", null, campaign: "replyToComment");
            message.Html = html;

        }

        public void AddSubject(ISendGrid message)
        {
            message.Subject = EmailResource.ReplyToCommentSubject;
        }
    }
}
