using System;
using System.Collections.Generic;
using SendGrid;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class ForgotPasswordMail : IMailBuilder
    {
        const string Category = "Password Recovery";
        const string Subject = "Password Recovery";


        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
           
            var forgotParams2 = parameters as ForgotPasswordMailParams2;
            if (forgotParams2 == null)
            {
                throw new NullReferenceException("forgotParams2");
            }

            
            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.ResetPwd");
           
            message.Subject = Subject;
            message.AddSubstitution("{NEW-PWD}", new List<string> { forgotParams2.Code });
            message.AddSubstitution("{CHANGE-URL}", new List<string> { forgotParams2.Link });
        }
    }
}
