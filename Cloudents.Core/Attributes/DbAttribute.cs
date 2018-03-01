using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Attributes
{
    [AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = false)]
    public class DbAttribute : Attribute
    {
        public DbAttribute(Database database)
        {
            Database = database;
        }

        public Database Database { get; private set; }
    }
}