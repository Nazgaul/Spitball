
using System;
using System.Collections.Generic;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Query;
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetItemQuery : QueryBase, IQueryCache
    {
        public GetItemQuery(long userId, long itemId, long boxId)
            : base(userId)
        {
            ItemId = itemId;
            BoxId = boxId;
        }

        public long BoxId { get; private set; }
        public long ItemId { get; private set; }


        string IQueryCache.CacheKey
        {
            get { return ItemId.ToString(CultureInfo.InvariantCulture); }
        }

        string IQueryCache.CacheRegion
        {
            get { return "Item"; }
        }


        public TimeSpan Expiration
        {
            get { return TimeSpan.FromHours(2); }
        }
    }
}
