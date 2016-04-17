using System.Collections.Generic;
using SendGrid;
using System;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class ChangeEmailMail : IMailBuilder
    {
        private const string Category = "Change Email";
        private const string Subject = "Change Email";


        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            var changeMailParams = parameters as ChangeEmailMailParams;
            if (changeMailParams == null)
            {
                throw new NullReferenceException("changeMailParams");
            }


            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.ChangeEmail");
           
            message.AddSubstitution("{CODE}", new List<string> { changeMailParams.Code });
        }

        public void AddSubject(ISendGrid message)
        {
            message.Subject = Subject;
        }
    }
}
