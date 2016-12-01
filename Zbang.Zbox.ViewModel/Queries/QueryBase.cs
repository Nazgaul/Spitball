﻿

namespace Zbang.Zbox.ViewModel.Queries
{
    public class QueryBase
    {
        public QueryBase(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; private set; }
    }

    public class QueryBaseUserId
    {
        public QueryBaseUserId(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; private set; }
    }
}
