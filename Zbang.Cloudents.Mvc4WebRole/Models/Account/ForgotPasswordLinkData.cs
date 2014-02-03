using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
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