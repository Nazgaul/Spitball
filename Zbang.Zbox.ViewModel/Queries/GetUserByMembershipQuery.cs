using System;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetUserByMembershipQuery
    {
        public GetUserByMembershipQuery(Guid membershipId)
        {
            MembershipId = membershipId;
        }

        public Guid MembershipId { get; private set; }
    }
}
