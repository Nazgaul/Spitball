﻿using System;

namespace Zbang.Cloudents.Mobile.Models.Account
{
    [Serializable]
    public class ForgotPasswordLinkData
    {
        protected ForgotPasswordLinkData()
        {

        }

        public ForgotPasswordLinkData(Guid membershipUserId, int step)
        {
            MembershipUserId = membershipUserId;
            Date = DateTime.UtcNow;
            Step = step;
        }

        public Guid MembershipUserId { get; private set; }
        public DateTime Date { get; private set; }
        public int Step { get; set; }
    }
}