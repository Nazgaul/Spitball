using System;

namespace Cloudents.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ModuleRegistrationAttribute : Attribute
    {
        public ModuleRegistrationAttribute(Enum.System system)
        {
            System = system;
        }

        public Enum.System System { get; private set; }

        public int Order { get; set; }
    }
}