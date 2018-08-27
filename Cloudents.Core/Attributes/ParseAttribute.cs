using System;

namespace Cloudents.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]

    public sealed class ParseAttribute : Attribute
    {
        public ParseAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }


    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public sealed class PublicValueAttribute : Attribute
    {
        public PublicValueAttribute()
        {
        }

    }
}
