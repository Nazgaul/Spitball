using System;
using SendGrid;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public abstract class MarketingMail : IMailBuilder
    {
        private readonly MarketingMailParams m_MarketingMailParams;
        protected MarketingMail(MailParameters parameters)
        {
            m_MarketingMailParams = parameters as MarketingMailParams;
        }

        public string GenerateMail()
        {
            //marketingMailParams = parameters as MarketingMailParams;
            //if (marketingMailParams == null)
            //{
            //    throw new NullReferenceException(nameof(marketingMailParams));
            //}

           // message.SetCategory(CategoryName);
            var html = LoadMailTempate.LoadMailFromContentWithDot(m_MarketingMailParams.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.MarketingTemplate");
            html.Replace("{name}", m_MarketingMailParams.Name);
            html.Replace("{body}", Text.Replace("\n", "<br><br>"));
           // message.EnableGoogleAnalytics("cloudentsMail", "email", null, campaign: CategoryName);
            return html.ToString();
        }

        protected abstract string Text { get; }
        protected abstract string CategoryName { get; }
        public abstract string AddSubject();
        public string AddCategory()
        {
            return CategoryName;
        }
    }
}