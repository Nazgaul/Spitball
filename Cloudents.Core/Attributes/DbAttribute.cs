using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DbAttribute : Attribute
    {
        public DbAttribute(Database database)
        {
            Database = database;
        }

        public Database Database { get; private set; }
    }
}