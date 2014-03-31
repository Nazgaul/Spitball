using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class InvitationToCloudentsMail : IMailBuilder
    {
        const string Category = "InvitationCloudents";
        const string Subject = "Invite to Cloudents";

        public void GenerateMail(SendGridMail.ISendGrid message, MailParameters parameters)
        {
            var inviteToCloudentsParams = parameters as InvitationToCloudentsMailParams;
            Zbang.Zbox.Infrastructure.Exceptions.Throw.OnNull(inviteToCloudentsParams, "inviteParams");

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.InviteCloudents");
            message.Subject = Subject;

            message.Html = message.Html.Replace("{USERNAME}", inviteToCloudentsParams.SenderName);
            message.Html = message.Html.Replace("{Image}", inviteToCloudentsParams.SenderImage);

        }
    }
}
