using System;

namespace Zbang.Zbox.Infrastructure.Extensions
{
    [AttributeUsage(AttributeTargets.All)]
    public class EnumDescription : Attribute
    {
        public EnumDescription(string description)
        {
            Description = description;
        }
        public EnumDescription(Type resourceType, string resourceName)
        {
            ResourceType = resourceType;
            ResourceName = resourceName;
        }
        public string Description { get; set; }

        public Type ResourceType { get; set; }
        public string ResourceName { get; set; }
    }
}
