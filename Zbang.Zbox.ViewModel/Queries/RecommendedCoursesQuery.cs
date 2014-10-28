using System;
using System.Collections.Generic;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries
{
   public class RecommendedCoursesQuery : IQueryCache
    {
       public RecommendedCoursesQuery(long universityId)
       {
           UniversityId = universityId;
       }

       public long UniversityId { get; private set; }

       public string CacheKey
       {
           get { return UniversityId.ToString(CultureInfo.InvariantCulture); }
       }

       public string CacheRegion
       {
           get { return "RecommendedCourses"; }
       }

       public List<string> CacheTags
       {
           get { return new List<string>(); }
       }

       public TimeSpan Expiration
       {
           get { return TimeSpan.FromHours(1); }
       }
    }
}
