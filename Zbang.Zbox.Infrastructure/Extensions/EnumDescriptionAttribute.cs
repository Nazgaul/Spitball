using System;

namespace Zbang.Zbox.Infrastructure.Extensions
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class EnumDescriptionAttribute : Attribute
    {
        public EnumDescriptionAttribute(string description)
        {
            Description = description;
        }
        public EnumDescriptionAttribute(Type resourceType, string resourceName)
        {
            ResourceType = resourceType;
            ResourceName = resourceName;
        }
        public string Description { get;private set; }

        public Type ResourceType { get; private set; }
        public string ResourceName { get; private set; }
    }
}
