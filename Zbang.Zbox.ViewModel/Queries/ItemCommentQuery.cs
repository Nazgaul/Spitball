using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class ItemCommentQuery : IQueryCache
    {
        public ItemCommentQuery(long itemId)
        {
            ItemId = itemId;
        }

        public long ItemId { get; }
        public string CacheKey => "x";
        public CacheRegions CacheRegion => CacheRegions.BuildItemCommentRegion(ItemId);
        public TimeSpan Expiration => TimeSpan.FromDays(1);
    }
}