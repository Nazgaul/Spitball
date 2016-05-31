using System;
using SendGrid;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public abstract class MarketingMail : IMailBuilder
    {
        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            var marketingMailParams = parameters as MarketingMailParams;
            if (marketingMailParams == null)
            {
                throw new NullReferenceException(nameof(marketingMailParams));
            }


            message.SetCategory(CategoryName);
            var html = LoadMailTempate.LoadMailFromContentWithDot(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.MarketingTemplate");
            html.Replace("{name}", marketingMailParams.Name);
            html.Replace("{body}", Text.Replace("\n", "<br><br>"));
            message.EnableGoogleAnalytics("cloudentsMail", "email", null, campaign: CategoryName);
            message.Html = html.ToString();
        }
        protected abstract string Text { get; }
        protected abstract string CategoryName { get; }
        public abstract void AddSubject(ISendGrid message);
    }

    
}