using System;

namespace Cloudents.Core.Models
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CacheAttribute : Attribute
    {
        public CacheAttribute(int duration, string region)
        {
            Duration = duration;
            Region = region;
        }

        public int Duration { get; set; }

        public string Region { get; }
    }
}
