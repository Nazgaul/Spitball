using System;
using System.Collections.Generic;
using SendGrid;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class WelcomeMail : IMailBuilder
    {
        private const string Category = "Welcome";


        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            var welcomeParams = parameters as WelcomeMailParams;
            if (welcomeParams == null)
            {
                throw new NullReferenceException("welcomeParams");
            }

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.Welcome");
           
            message.AddSubstitution("{Name}", new List<string> { welcomeParams.Name });

            message.EnableGoogleAnalytics("cloudentsMail", "email", null, campaign: "welcomeEmail");
        }

        public void AddSubject(ISendGrid message)
        {
            message.Subject = EmailResource.WelcomeSubject; 
        }
    }
}
