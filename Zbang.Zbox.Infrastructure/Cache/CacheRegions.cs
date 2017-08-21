namespace Zbang.Zbox.Infrastructure.Cache
{
    public sealed class CacheRegions
    {
        public string Region { get; }
        public bool SuppressVersion { get;  }

        public CacheRegions(string region, bool suppressVersion)
        {
            Region = region;
            SuppressVersion = suppressVersion;
        }

        public static CacheRegions BuildFeedRegion(long boxId)
        {
            return new CacheRegions("feed_" + boxId, false);

            //return $"feed_{boxId}";
        }

        //public const string UniversityRegion = "university";
       // public const string BoxRegion = "box";

        public static CacheRegions BuildNodesRegion(long universityId)
        {
            return new CacheRegions("university_" + universityId, false);
            //return $"university_{universityId}";
        }

        public static CacheRegions BuildItemCommentRegion(long itemId)
        {
            return new CacheRegions("itemComment_" + itemId, false);
           // return $"itemComment_{itemId}";
        }

        public static readonly CacheRegions ZipToLocation = new CacheRegions("zip", true);
        public static readonly CacheRegions University = new CacheRegions("university",false);
        public static readonly CacheRegions Search = new CacheRegions("search", false);
        public static readonly CacheRegions Homepage = new CacheRegions("boxesHomePage", false);
        public static readonly CacheRegions Profile = new CacheRegions("profileStats", false);
        public static readonly CacheRegions SearchMobile = new CacheRegions("searchMobile", false);

        public override string ToString()
        {
            return Region;
        }
    }
}
