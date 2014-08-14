using System;
using System.Text;
using Zbang.Zbox.Infrastructure.Mail.EmailParameters;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class StoreContactUsMail : IMailBuilder
    {
        public void GenerateMail(SendGrid.ISendGrid message, MailParameters parameters)
        {
            var contactUs = parameters as StoreContactUs;
            if (contactUs == null)
            {
                throw new NullReferenceException("partnersParams");
            }
            var sb = new StringBuilder(LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.Store.ContactUsSubmission"));
            message.Subject = "Store contact us request";
            message.SetCategory("StoreContactUs");
            sb.Replace("{USERNAME}", contactUs.Name)
            .Replace("{TELEPHONE}", contactUs.Phone)
            .Replace("{SCHOOL}", contactUs.University)
            .Replace("{EMAIL}", contactUs.Email)
            .Replace("{MESSAGE}", contactUs.Text);

            message.Html = sb.ToString();
        }
    }
}
