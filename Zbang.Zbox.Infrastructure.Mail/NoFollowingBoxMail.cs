using SendGrid;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class NoFollowingBoxMail : MarketingMail
    {
        protected override string Text => EmailResource.NoFollowBoxText;
        protected override string CategoryName => "No Follow Box";

        public override string AddSubject()
        {
            return EmailResource.NoFollowBoxSubject;
        }

        public NoFollowingBoxMail(MailParameters parameters) : base(parameters)
        {
        }
    }


    public class LowContributionMail : MarketingMail
    {
        protected override string Text => EmailResource.LowContributionText;
        protected override string CategoryName => "Low Contribution";

        public override string AddSubject()
        {
            return EmailResource.LowContributionSubject;
        }

        public LowContributionMail(MailParameters parameters) : base(parameters)
        {
        }
    }
}