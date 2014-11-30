using System;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Query;
namespace Zbang.Zbox.ViewModel.Queries
{


    public class GetUserFriendsQuery : QueryBase, IQueryCache
    {
        public GetUserFriendsQuery(long id)
            : base(id)
        {
        }

        public string CacheKey
        {
            get { return UserId.ToString(CultureInfo.InvariantCulture); }
        }

        public string CacheRegion
        {
            get { return "Friend"; }
        }


        public TimeSpan Expiration
        {
            get { return TimeSpan.FromMinutes(10); }
        }
    }
}
