
using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Query;
namespace Zbang.Zbox.ViewModel.Queries.Library
{
    public class GetUniversityDetailQuery : QueryBase , IQueryCache
    {
        public GetUniversityDetailQuery(long universityId)
            : base(universityId)
        {
           
        }


        public string CacheKey
        {
            get { return string.Format("{0}", UserId); }
        }

        public string CacheRegion
        {
            get { return "UniversityDetail"; }
        }

        public TimeSpan Expiration
        {
            get { return TimeSpan.FromMinutes(60); }
        }
    }
}
