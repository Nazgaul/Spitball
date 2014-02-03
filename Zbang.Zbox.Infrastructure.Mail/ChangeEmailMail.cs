using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class ChangeEmailMail : IMailBuilder
    {
        const string Category = "Change Email";
        const string Subject = "Change Email";


        public void GenerateMail(SendGridMail.ISendGrid message, MailParameters parameters)
        {
            var changeMailParams = parameters as ChangeEmailMailParams;
            Zbang.Zbox.Infrastructure.Exceptions.Throw.OnNull(changeMailParams, "ChangeEmailMailParams");

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.ChangeEmail");
            //message.Text = textBody;
            message.Subject = Subject;
            message.AddSubVal("{CODE}", new List<string> { changeMailParams.Code });
        }
    }
}
