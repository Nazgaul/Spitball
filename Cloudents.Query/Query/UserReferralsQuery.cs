using Cloudents.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Query.Query
{
    public class UserReferralsQuery : IQuery<UserReferralsDto>
    {
        public UserReferralsQuery(long id)
        {
            Id = id;
        }

        public long Id { get; private set; }
    }
}
