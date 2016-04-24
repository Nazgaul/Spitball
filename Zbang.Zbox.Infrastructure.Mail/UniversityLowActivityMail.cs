﻿using SendGrid;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class UniversityLowActivityMail : MarketingMail
    {
        protected override string Text => EmailResource.UniversityLowActivityText;
        protected override string CategoryName => "university low activity";
        public override void AddSubject(ISendGrid message)
        {
            message.Subject = EmailResource.UniversityLowActivitySubject;
        }
    }
}