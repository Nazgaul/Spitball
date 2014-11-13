using System;
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
            message.Subject = Subject;

            message.Html = message.Html.Replace("{ItemUrl}", flagItemsParams.Url);
            message.Html = message.Html.Replace("{ITEM NAME}", flagItemsParams.ItemName);
            message.Html = message.Html.Replace("{REASON}", flagItemsParams.Reason);
            message.Html = message.Html.Replace("{FLAGGER-USERNAME}", flagItemsParams.UserName);
            message.Html = message.Html.Replace("{FLAGGER-EMAIL}", flagItemsParams.Email);
            
        }
    }
}
