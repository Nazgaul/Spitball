using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class NoUniversityMail : MarketingMail
    {
       protected override string Text => EmailResource.NoUniversityText;
        protected override string CategoryName => "No University";

        public override string AddSubject()
        {
           return EmailResource.NoUniversitySubject;
        }

        public NoUniversityMail(MailParameters parameters) : base(parameters)
        {
        }
    }
}
