﻿using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetItemQuery : QueryBase
    {
        public GetItemQuery(long userId, long itemId, long boxId)
            : base(userId)
        {
            ItemId = itemId;
            BoxId = boxId;
        }

        public long BoxId { get; private set; }
        public long ItemId { get; }

    }

    public class ItemCommentQuery : IQueryCache
    {
        public ItemCommentQuery(long itemId)
        {
            ItemId = itemId;
        }

        public long ItemId { get; }
        public string CacheKey => "x";
        public string CacheRegion => CacheRegions.BuildItemCommentRegion(ItemId);
        public TimeSpan Expiration => TimeSpan.FromDays(28);
    }
}
