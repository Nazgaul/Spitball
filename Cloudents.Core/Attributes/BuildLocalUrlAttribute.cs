using System;

namespace Cloudents.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
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

        public string ListObjectName { get; }

        public int? SizeOfPage { get; }

        public string PageArgumentName { get; }
    }
}