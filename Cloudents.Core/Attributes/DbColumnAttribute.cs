using System;

namespace Cloudents.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DbColumnAttribute : Attribute
    {
        public string Name { get;  }

        public DbColumnAttribute(string name)
        {
            Name = name;
        }
    }
}