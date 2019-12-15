using System;

namespace Cloudents.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CacheAttribute : Attribute
    {
        /// <summary>
        /// Initialize Cache
        /// </summary>
        /// <param name="duration">Duration in seconds</param>
        /// <param name="region">Specific region to store</param>
        /// <param name="slide">Sliding or absolute</param>
        public CacheAttribute(int duration, string region, bool slide)
        {
            Duration = duration;
            Region = region;
            Slide = slide;
        }

        /// <summary>
        /// Duration In seconds
        /// </summary>
        public int Duration { get; }

        public string Region { get; }

        public bool Slide { get; private set; }
    }
}
