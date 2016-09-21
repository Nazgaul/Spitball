using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class LowCoursesActivityMail : MarketingMail
    {
        protected override string Text => EmailResource.CoursesLowActivityText;
        protected override string CategoryName => "courses low activity";
        public override string AddSubject()
        {
            return EmailResource.CoursesLowActivitySubject;
        }

        public LowCoursesActivityMail(MailParameters parameters) : base(parameters)
        {
        }
    }
}