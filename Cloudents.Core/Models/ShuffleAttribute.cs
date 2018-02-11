using System;

namespace Cloudents.Core.Models
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ShuffleAttribute : Attribute
    {
        public ShuffleAttribute(string listObjectName)
        {
            ListObjectName = listObjectName;
        }


        string ListObjectName { get; }

    }
}