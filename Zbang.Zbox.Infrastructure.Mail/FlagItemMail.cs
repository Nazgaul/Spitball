using System;
using System.Collections.Generic;
using SendGrid;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class FlagItemMail : IMailBuilder
    {

        const string Category = "Flag Bad Item";
        const string Subject = "Flag Bad Item";


        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            var flagItemsParams = parameters as FlagItemMailParams;
            if (flagItemsParams == null)
            {
                throw new NullReferenceException("flagItemsParams");
            }

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.FlaggedItem");
            //message.Text = textBody;
            message.Subject = Subject;
            message.AddSubstitution("{ItemUrl}", new List<string> { flagItemsParams.Url });
            message.AddSubstitution("{ITEM NAME}", new List<string> { flagItemsParams.ItemName });
            message.AddSubstitution("{REASON}", new List<string> { flagItemsParams.Reason });
            message.AddSubstitution("{FLAGGER-USERNAME}", new List<string> { flagItemsParams.UserName });
            message.AddSubstitution("{FLAGGER-EMAIL}", new List<string> { flagItemsParams.Email });
            
        }
    }
}
