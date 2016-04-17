using System;
using SendGrid;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class DepartmentApprovedAccessMail : IMailBuilder
    {
        private const string Category = "Request approved";
        private const string Subject = "Approved access";


        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            var departmentApprovedAccessParams = parameters as DepartmentApprovedMailParams;
            if (departmentApprovedAccessParams == null)
            {
                throw new NullReferenceException("departmentApprovedAccessParams");
            }

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.DepartmentApprovedAccess");
            

            message.EnableGoogleAnalytics("cloudentsMail", "email", null, campaign: "approved Access");
            message.Html = message.Html.Replace("{DEP_NAME}", departmentApprovedAccessParams.DepName);
        }

        public void AddSubject(ISendGrid message)
        {
            message.Subject = Subject;
        }
    }
}