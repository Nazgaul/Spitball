using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
