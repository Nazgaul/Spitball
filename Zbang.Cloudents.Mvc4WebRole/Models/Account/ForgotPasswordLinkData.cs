using System;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    [Serializable]
    public class ForgotPasswordLinkData
    {
        protected ForgotPasswordLinkData()
        {

        }

        public ForgotPasswordLinkData(Guid membershipUserId, int step, string hash)
        {
            MembershipUserId = membershipUserId;
            Step = step;
            Hash = hash;
        }

        public Guid MembershipUserId { get; private set; }
        public int Step { get; set; }

        public string Hash { get; private set; }
    }
}