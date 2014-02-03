using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class WelcomeMail : IMailBuilder
    {
        const string Category = "Welcome";
        const string Subject = "Welcome to Cloudents";


        public void GenerateMail(SendGridMail.ISendGrid message, MailParameters parameters)
        {
            var welcomeParams = parameters as WelcomeMailParams;
            Zbang.Zbox.Infrastructure.Exceptions.Throw.OnNull(welcomeParams, "welcomeParams");

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.Welcome");
            //message.Text = textBody;
            message.Subject = Subject;
            message.AddSubVal("{Name}", new List<string> { welcomeParams.Name });

        }
    }
}
