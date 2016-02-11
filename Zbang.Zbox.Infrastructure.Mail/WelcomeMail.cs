using System;
using System.Collections.Generic;
using SendGrid;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class WelcomeMail : IMailBuilder
    {
        const string Category = "Welcome";
        const string Subject = "Welcome to Spitball";


        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            var welcomeParams = parameters as WelcomeMailParams;
            if (welcomeParams == null)
            {
                throw new NullReferenceException("welcomeParams");
            }

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.Welcome");
            message.Subject = Subject;
            message.AddSubstitution("{Name}", new List<string> { welcomeParams.Name });

            message.EnableGoogleAnalytics("cloudentsMail", "email", null, campaign: "welcomeEmail");
        }
    }

    internal class DepartmentRequestAccessMail : IMailBuilder
    {
        const string Category = "Request access";
        const string Subject = "Someone request access";


        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            var departmentRequestAccessParams = parameters as DepartmentRequestAccessMailParams;
            if (departmentRequestAccessParams == null)
            {
                throw new NullReferenceException("departmentRequestAccessParams");
            }

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.RequestDepartmentAccess");
            message.Subject = Subject;

            message.EnableGoogleAnalytics("cloudentsMail", "email", null, campaign: "requestAccess");
        }
    }
}
