using System;
using SendGrid;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class InvitationToCloudentsMail : IMailBuilder
    {
        private const string Category = "InvitationCloudents";
        private const string Subject = "Invite to Spitball";

        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            var inviteToCloudentsParams = parameters as InvitationToCloudentsMailParams;
            if (inviteToCloudentsParams == null)
            {
                throw new NullReferenceException("inviteToCloudentsParams");
            }
            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.InviteCloudents");
            
            message.EnableGoogleAnalytics("cloudentsMail", "email", null, campaign: "InvitationCloudents");
            message.Html = message.Html.Replace("{USERNAME}", inviteToCloudentsParams.SenderName);
            message.Html = message.Html.Replace("{Image}", inviteToCloudentsParams.SenderImage);
            message.Html = message.Html.Replace("{{Url}}", inviteToCloudentsParams.Url);

        }

        public void AddSubject(ISendGrid message)
        {
            message.Subject = Subject;
        }
    }
}
