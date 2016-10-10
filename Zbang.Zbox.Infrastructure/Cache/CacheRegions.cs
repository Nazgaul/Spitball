namespace Zbang.Zbox.Infrastructure.Cache
{
    public static class CacheRegions
    {
        public static string BuildFeedRegion(long boxId)
        {
            return "feed_" + boxId;
            //return $"feed_{boxId}";
        }

        public const string UniversityRegion = "university";
        public const string BoxRegion = "box";

        public static string BuildNodesRegion(long universityId)
        {
            return "university_" + universityId;
            //return $"university_{universityId}";
        }

        public static string BuildItemCommentRegion(long itemId)
        {
            return "itemComment_" + itemId;
           // return $"itemComment_{itemId}";
        }


    }
}
