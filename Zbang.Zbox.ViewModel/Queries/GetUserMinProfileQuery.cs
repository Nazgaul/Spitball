using System;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetUserMinProfileQuery
    {
        public GetUserMinProfileQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; }
    }
}
