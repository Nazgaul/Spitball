using System;

namespace Cloudents.Core.Models
{
    public sealed class CacheRegion
    {
        public string Name { get; }

        private CacheRegion(string name)
        {
            Name = name;
        }

        public static readonly CacheRegion Ai = new CacheRegion("ai");
        public static readonly CacheRegion SearchCse = new CacheRegion("search-cse");
    }

    public class CacheResultAttribute : Attribute
    {
        public CacheResultAttribute(int duration, string region)
        {
            Duration = duration;
            Region = region;
        }

        public int Duration { get; set; }

        public string Region { get; }
    }
}
