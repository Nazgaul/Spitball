using System;

namespace Cloudents.Core.Models
{
    [AttributeUsageAttribute(AttributeTargets.Method, AllowMultiple = false)]
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
