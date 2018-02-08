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

    public sealed class ShuffleAttribute : Attribute
    {
        public ShuffleAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; private set; }

    }

    public sealed class BuildLocalUrlAttribute : Attribute
    {
        public BuildLocalUrlAttribute(string listObjectName, int sizeOfPage, string pageArgumentName)
        {
            ListObjectName = listObjectName;
            SizeOfPage = sizeOfPage;
            PageArgumentName = pageArgumentName;
        }

        public BuildLocalUrlAttribute(string listObjectName)
        {
            ListObjectName = listObjectName;
        }

        public string ListObjectName { get; private set; }

        public int? SizeOfPage { get; set; }

        public string PageArgumentName { get; set; }
        
    }
}
