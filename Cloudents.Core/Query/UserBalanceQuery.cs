using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    public class UserBalanceQuery : IQuery<IEnumerable<BalanceDto>>
    {
        public UserBalanceQuery(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; }
    }
}