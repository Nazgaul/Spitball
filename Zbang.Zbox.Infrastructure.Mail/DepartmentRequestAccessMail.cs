using System;
using SendGrid;

namespace Zbang.Zbox.Infrastructure.Mail
{
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