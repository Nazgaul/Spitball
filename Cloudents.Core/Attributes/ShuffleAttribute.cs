using System;

namespace Cloudents.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ShuffleAttribute : Attribute
    {
        public ShuffleAttribute(string listObjectName)
        {
            ListObjectName = listObjectName;
        }

        public ShuffleAttribute()
        {
                
        }

        public string ListObjectName { get; }
    }
}