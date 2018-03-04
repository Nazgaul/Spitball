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

    [AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = true)]
    public class ModuleRegistrationAttribute : Attribute
    {
        public ModuleRegistrationAttribute(Enum.System system)
        {
            System = system;
        }

        public Enum.System System { get; private set; }
    }
}