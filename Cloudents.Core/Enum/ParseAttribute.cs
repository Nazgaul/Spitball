using System;

namespace Cloudents.Core.Enum
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
}
