using System;
using System.Globalization;
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
            var sb = new StringBuilder(LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.Partners.WeeklyUpdate"));
            message.Subject = "Store contact us request";
            message.SetCategory("StoreContactUs");
            sb.Replace("{7DAYSUSERS}", contactUs.Name)
            .Replace("{TOTALUSERS}", contactUs.Phone)
            .Replace("{7DAYSCOURSES}", contactUs.University)
            .Replace("{TOTALCOURSES}", contactUs.Email)
            .Replace("{7DAYSITEMS}", contactUs.Text);

            message.Html = sb.ToString();
        }
    }
}
