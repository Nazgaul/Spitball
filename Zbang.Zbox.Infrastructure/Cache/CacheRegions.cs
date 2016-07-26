﻿namespace Zbang.Zbox.Infrastructure.Cache
{
    public static class CacheRegions
    {
        public static string BuildFeedRegion(long boxId)
        {
            return $"feed_{boxId}";
        }

        public const string UniversityRegion = "university";

        public static string BuildNodesRegion(long universityId)
        {
            return $"university_{universityId}";
        }
    }
}
