
using System;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries.Boxes
{
    public class GetBoxesQuery : QueryBase, IQueryCache
    {
        public GetBoxesQuery(long id)
            : base(id)
        {

        }





        public string CacheKey
        {
            get { return this.UserId.ToString(); }
        }

        public string CacheRegion
        {
            get { return "Boxes"; }
        }

        public System.Collections.Generic.List<string> CacheTags
        {
            get { return null; }
        }

        public TimeSpan Expiration
        {
            get { return TimeSpan.FromMinutes(5); }
        }
    }


}
