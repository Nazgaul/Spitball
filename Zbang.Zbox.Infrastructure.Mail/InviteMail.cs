using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class InviteMail : IMailBuilder
    {
        const string Category = "Invite";
        const string Subject = "Invitation to Cloudents";


        public void GenerateMail(SendGridMail.ISendGrid message, MailParameters parameters)
        {
            var inviteParams = parameters as InviteMailParams;
            Zbang.Zbox.Infrastructure.Exceptions.Throw.OnNull(inviteParams, "inviteParams");

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.InviteBox");
            //message.Text = textBody;
            message.Subject = Subject;
            message.AddSubVal("{INVITOR}", new List<string> { inviteParams.Invitor });
            message.AddSubVal("{BOX NAME}", new List<string> { inviteParams.BoxName });
            message.AddSubVal("{PERSONAL_MSG}", new List<string> { string.Empty });
            message.AddSubVal("{BoxUrl}", new List<string> { inviteParams.BoxUrl });
        }
    }
}
