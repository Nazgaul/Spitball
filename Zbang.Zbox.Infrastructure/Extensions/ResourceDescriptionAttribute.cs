using System;

namespace Zbang.Zbox.Infrastructure.Extensions
{
    [AttributeUsage(AttributeTargets.Enum| AttributeTargets.Field)]
    public sealed class ResourceDescriptionAttribute : Attribute
    {
        public ResourceDescriptionAttribute(string description)
        {
            Description = description;
        }
        public ResourceDescriptionAttribute(Type resourceType, string resourceName)
        {
            ResourceType = resourceType;
            ResourceName = resourceName;
        }
        public string Description { get;private set; }

        public Type ResourceType { get; private set; }
        public string ResourceName { get; private set; }
    }
}
