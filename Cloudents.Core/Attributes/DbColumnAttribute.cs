using System;

namespace Cloudents.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DbColumnAttribute : Attribute
    {
        public string Name { get;  }

        public DbColumnAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AdminCommandHandler : Attribute
    {

    }
}