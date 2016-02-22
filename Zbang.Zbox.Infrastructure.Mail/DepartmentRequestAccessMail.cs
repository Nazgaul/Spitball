using System;
using SendGrid;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class DepartmentRequestAccessMail : IMailBuilder
    {
        const string Category = "Request access";
        const string Subject = "Another spitballer requests access";


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
            message.Html = message.Html.Replace("{USER_NAME}", departmentRequestAccessParams.UserName);
            message.Html = message.Html.Replace("{USER_IMG}", departmentRequestAccessParams.UserImage);
            message.Html = message.Html.Replace("{DEP_NAME}", departmentRequestAccessParams.DepName);
        }
    }
}