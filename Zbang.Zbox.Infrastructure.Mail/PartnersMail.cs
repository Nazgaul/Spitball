using System;
using System.Globalization;
using System.Text;
using SendGrid;
using Zbang.Zbox.Infrastructure.Mail.EmailParameters;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class PartnersMail : IMailBuilder
    {
        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            var partnersParams = parameters as Partners;
            if (partnersParams == null)
            {
                throw new NullReferenceException("partnersParams");
            }
            var sb = new StringBuilder(LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.Partners.WeeklyUpdate"));
            message.Subject = "Cloudents weekly status update";
            message.SetCategory("Partners");
            sb.Replace("{7DAYSUSERS}", partnersParams.WeekUsers.ToString(CultureInfo.InvariantCulture))
              .Replace("{TOTALUSERS}", partnersParams.AllUsers.ToString(CultureInfo.InvariantCulture))
              .Replace("{7DAYSCOURSES}", partnersParams.WeekCourses.ToString(CultureInfo.InvariantCulture))
              .Replace("{TOTALCOURSES}", partnersParams.AllCourses.ToString(CultureInfo.InvariantCulture))
              .Replace("{7DAYSITEMS}", partnersParams.WeekItems.ToString(CultureInfo.InvariantCulture))
              .Replace("{TOTALITEMS}", partnersParams.AllItems.ToString(CultureInfo.InvariantCulture))
              .Replace("{7DAYSQNA}", partnersParams.WeekQnA.ToString(CultureInfo.InvariantCulture))
              .Replace("{TOTALQNA}", partnersParams.AllQnA.ToString(CultureInfo.InvariantCulture))
              .Replace("{SCHOOLNAME}", partnersParams.SchoolName);

            int i = 0;
            foreach (var university in partnersParams.Universities)
            {
                sb.Replace("{USER" + i + "NAME}", university.Name)
                  .Replace("{USER" + i + "COUNT}", university.StudentsCount.ToString(CultureInfo.InvariantCulture));
                i++;
            }

            message.Html = sb.ToString();
        }
    }
}
