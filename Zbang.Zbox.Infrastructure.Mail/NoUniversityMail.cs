using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class NoUniversityMail : IMailBuilder
    {
        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            var noUniversityMailParams = parameters as NoUniversityMailParams;
            if (noUniversityMailParams == null)
            {
                throw new NullReferenceException(nameof(noUniversityMailParams));
            }


            message.SetCategory("No University");
            var html = LoadMailTempate.LoadMailFromContentWithDot(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.MarketingTemplate");
            html.Replace("{name}", noUniversityMailParams.Name);
            html.Replace("{body}", EmailResource.NoUniversityText);

            message.Html = html.ToString();
        }
    }
}
