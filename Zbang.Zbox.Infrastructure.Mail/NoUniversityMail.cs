using System;
using SendGrid;
using Zbang.Zbox.Infrastructure.Mail.Resources;

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

            message.Html = html.ToString();
        }
        protected abstract string Text { get; }
        protected abstract string CategoryName { get; }
        public abstract void AddSubject(ISendGrid message);
    }
    public class NoUniversityMail : MarketingMail
    {
       protected override string Text => EmailResource.NoUniversityText;
        protected override string CategoryName => "No University";

        public override void AddSubject(ISendGrid message)
        {
            message.Subject = EmailResource.NoUniversitySubject;
        }
    }

    public class NoFollowingBoxMail : MarketingMail
    {
        protected override string Text => EmailResource.NoFollowBoxText;
        protected override string CategoryName => "No Follow Box";

        public override void AddSubject(ISendGrid message)
        {
            message.Subject = EmailResource.NoFollowBoxSubject;
        }
    }

    public class UniversityLowActivityMail : MarketingMail
    {
        protected override string Text => EmailResource.UniversityLowActivityText;
        protected override string CategoryName => "university low activity";
        public override void AddSubject(ISendGrid message)
        {
            message.Subject = EmailResource.UniversityLowActivitySubject;
        }
    }

    public class LowCoursesActivityMail : MarketingMail
    {
        protected override string Text => EmailResource.CoursesLowActivityText;
        protected override string CategoryName => "courses low activity";
        public override void AddSubject(ISendGrid message)
        {
            message.Subject = EmailResource.CoursesLowActivitySubject;
        }
    }


}
