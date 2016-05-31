using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class LikesMail : IMailBuilder
    {
        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            var likesMailParams = parameters as LikesMailParams;
            if (likesMailParams == null)
            {
                throw new NullReferenceException(nameof(likesMailParams));
            }


            message.SetCategory("Likes mail");
            var html = LoadMailTempate.LoadMailFromContentWithDot(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.MarketingTemplate");
            html.Replace("{name}", likesMailParams.Name);

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(string.Format(EmailResource.LikesMailTitle, likesMailParams.LikeData.Count()));
            foreach (var likeData in likesMailParams.LikeData)
            {
                sb.AppendLine(string.Format(EmailResource.LikesMailLineDetails, likeData.UserName, likeData.Type.GetEnumDescription(), likeData.OnLikeText));
            }
            sb.AppendLine();
            sb.AppendLine(EmailResource.LikesMailFooter);
            html.Replace("{body}", sb.ToString().Replace("\n", "<br>"));
            message.EnableGoogleAnalytics("cloudentsMail", "email", null, campaign: "like");
            message.Html = html.ToString();
        }

        public void AddSubject(ISendGrid message)
        {
            message.Subject = EmailResource.LikesMailSubject;
        }
    }
}
