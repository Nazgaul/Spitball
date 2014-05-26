
using System;
using System.Collections.Generic;
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


        List<string> m_Tags = new List<string>();

        List<string> IQueryCache.CacheTags
        {
            get
            {
                return m_Tags;
            }
        }


    

        string IQueryCache.CacheKey
        {
            get { return ItemId.ToString(); }
        }

        string IQueryCache.CacheRegion
        {
            get { return "Item"; }
        }


        public System.TimeSpan Expiration
        {
            get { return TimeSpan.FromHours(2); }
        }
    }
}
