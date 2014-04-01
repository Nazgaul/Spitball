using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail.EmailParameters;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class PartnersMail : IMailBuilder
    {
        public void GenerateMail(SendGridMail.ISendGrid message, MailParameters parameters)
        {
            var partnersParams = parameters as Partners;
            Zbang.Zbox.Infrastructure.Exceptions.Throw.OnNull(partnersParams, "Partners");

            var sb = new StringBuilder(LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.Partners.WeeklyUpdate"));
            message.Subject = "Cloudents weekly status update";
            message.SetCategory("Partners");
            sb.Replace("{7DAYSUSERS}", partnersParams.WeekUsers.ToString())
              .Replace("{TOTALUSERS}", partnersParams.AllUsers.ToString())
              .Replace("{7DAYSCOURSES}", partnersParams.WeekCourses.ToString())
              .Replace("{TOTALCOURSES}", partnersParams.AllCourses.ToString())
              .Replace("{7DAYSITEMS}", partnersParams.WeekItems.ToString())
              .Replace("{TOTALITEMS}", partnersParams.AllItems.ToString())
              .Replace("{7DAYSQNA}", partnersParams.WeekQnA.ToString())
              .Replace("{TOTALQNA}", partnersParams.AllQnA.ToString())
              .Replace("{SCHOOLNAME}", partnersParams.SchoolName);

            int i = 0;
            foreach (var university in partnersParams.Universities)
            {
                sb.Replace("{USER" + i + "NAME}", university.Name)
                  .Replace("{USER" + i + "COUNT}", university.StudentsCount.ToString());
                i++;
            }

            message.Html = sb.ToString();
        }
    }
}
