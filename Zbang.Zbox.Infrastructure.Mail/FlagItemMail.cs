using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class FlagItemMail : IMailBuilder
    {

        const string Category = "Flag Bad Item";
        const string Subject = "Flag Bad Item";


        public void GenerateMail(SendGridMail.ISendGrid message, MailParameters parameters)
        {
            var flagItemsParams = parameters as FlagItemMailParams;
            Zbang.Zbox.Infrastructure.Exceptions.Throw.OnNull(flagItemsParams, "flagItemsParams");

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.FlaggedItem");
            //message.Text = textBody;
            message.Subject = Subject;
            message.AddSubVal("{ItemUrl}", new List<string> { flagItemsParams.Url });
            message.AddSubVal("{ITEM NAME}", new List<string> { flagItemsParams.ItemName });
            message.AddSubVal("{REASON}", new List<string> { flagItemsParams.Reason });
            message.AddSubVal("{FLAGGER-USERNAME}", new List<string> { flagItemsParams.UserName });
            message.AddSubVal("{FLAGGER-EMAIL}", new List<string> { flagItemsParams.Email });
            
        }
    }
}
