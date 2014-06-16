using System;
using SendGrid;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class InviteMail : IMailBuilder
    {
        const string Category = "Invite";


        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            var inviteParams = parameters as InviteMailParams;

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.InviteCourse");
            //message.Text = textBody;
            if (inviteParams == null)
            {
                throw new NullReferenceException("inviteParams");
            }
            message.Subject = string.Format(EmailResource.InviteSubject, inviteParams.BoxName);
            message.Html = message.Html.Replace("{INVITOR}", inviteParams.Invitor);
            message.Html = message.Html.Replace("{BOXNAME}", inviteParams.BoxName);
            message.Html = message.Html.Replace("{BoxUrl}", inviteParams.BoxUrl);
            message.Html = message.Html.Replace("{ImgUrl}", inviteParams.InvitorImage);
        }
    }
}
