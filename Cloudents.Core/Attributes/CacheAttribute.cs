using System;

namespace Cloudents.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CacheAttribute : Attribute
    {
        public CacheAttribute(int duration, string region, bool slide)
        {
            Duration = duration;
            Region = region;
            Slide = slide;
        }

        public int Duration { get; }

        public string Region { get; }

        public bool Slide { get; private set; }
    }
}
