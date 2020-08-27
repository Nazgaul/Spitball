using System;
using System.Collections.Generic;
using System.Globalization;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Message.Email
{
    public class SubscribeTutorMessage : BaseEmail
    {
        public string TutorFirstName { get; private set; }
        public string UserFirstName { get;private set; }
        public Money SubscriptionAmount { get; private set;}
        public string UserEmail { get;private set; }

        public SubscribeTutorMessage(string to, string tutorFirstName,string userFirstName,
            Money subscriptionAmount, string userEmail)
            :base(to,"Great news you have a subscriber on Spitball",null)
        {
            TutorFirstName = tutorFirstName;
            UserFirstName = userFirstName;
            SubscriptionAmount = subscriptionAmount;
            UserEmail = userEmail;
        }


        public override string? Campaign => null;
        public override UnsubscribeGroup UnsubscribeGroup => UnsubscribeGroup.Update;
        protected override IDictionary<CultureInfo, string>? Templates => null;

        public override string ToString()
        {
            return @$"Hey {TutorFirstName}

You have a new subscriber to your service on Spitball. {UserFirstName} has purchased a subscription for {SubscriptionAmount}.

If you wish to send them a personal welcome email here is email : {UserEmail}.

The Spitball Team
".Replace(Environment.NewLine,"<br>");
        }
    }
}