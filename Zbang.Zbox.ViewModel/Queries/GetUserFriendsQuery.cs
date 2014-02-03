

using System;
using Zbang.Zbox.Infrastructure.Query;
namespace Zbang.Zbox.ViewModel.Queries
{


    public class GetUserFriendsQuery : QueryBase, IQueryCache
    {
        public GetUserFriendsQuery(long Id)
            : base(Id)
        {
        }

        public string CacheKey
        {
            get { return base.UserId.ToString(); }
        }

        public string CacheRegion
        {
            get { return "Friend"; }
        }

        public System.Collections.Generic.List<string> CacheTags
        {
            get
            {
                return null;
            }
          
        }

        public System.TimeSpan Expiration
        {
            get { return TimeSpan.FromMinutes(10); }
        }
    }
}
