using SendGrid;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
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