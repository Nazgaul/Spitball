﻿using System;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetUserMinProfileQuery : IQueryCache
    {
        public GetUserMinProfileQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; private set; }

        public string CacheKey
        {
            get { return UserId.ToString(CultureInfo.InvariantCulture); }
        }

        public string CacheRegion
        {
            get { return "UserMinProfile"; }
        }

        public TimeSpan Expiration
        {
            get { return TimeSpan.FromMinutes(20); }
        }
    }
}
