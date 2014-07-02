
using System;
using System.Collections.Generic;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries.Boxes
{
    public class GetBoxesQuery : QueryBase//, IQueryCache
    {
        public GetBoxesQuery(long id)
            : base(id)
        {

        }





        //public string CacheKey
        //{
        //    get { return UserId.ToString(CultureInfo.InvariantCulture); }
        //}

        //public string CacheRegion
        //{
        //    get { return "Boxes"; }
        //}

        //public List<string> CacheTags
        //{
        //    get { return null; }
        //}

        //public TimeSpan Expiration
        //{
        //    get { return TimeSpan.FromMinutes(5); }
        //}
    }


}
