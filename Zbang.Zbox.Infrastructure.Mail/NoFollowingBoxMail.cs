using SendGrid;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class NoFollowingBoxMail : MarketingMail
    {
        protected override string Text => EmailResource.NoFollowBoxText;
        protected override string CategoryName => "No Follow Box";

        public override void AddSubject(ISendGrid message)
        {
            message.Subject = EmailResource.NoFollowBoxSubject;
        }
    }


    public class LowContributionMail : MarketingMail
    {
        protected override string Text => EmailResource.LowContributionText;
        protected override string CategoryName => "Low Contribution";

        public override void AddSubject(ISendGrid message)
        {
            message.Subject = EmailResource.LowContributionSubject;
        }
    }
}