
using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxSideBarQuery  //: IQueryCache
    {
        public GetBoxSideBarQuery(long boxId,long userId)
        {
            UserId = userId;
            BoxId = boxId;
        }

        public long BoxId { get; }

        public long UserId { get; private set; }
        //public string CacheKey => BoxId.ToString();
        //public string CacheRegion => CacheRegions.BoxRegion;
        //public TimeSpan Expiration => TimeSpan.FromDays(1);
    }
}
