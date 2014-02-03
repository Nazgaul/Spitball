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
        public EnumDescription(Type resorceType, string resourceName)
        {
            ResorceType = resorceType;
            ResourceName = resourceName;
        }
        public string Description { get; set; }

        public Type ResorceType { get; set; }
        public string ResourceName { get; set; }
    }
}
