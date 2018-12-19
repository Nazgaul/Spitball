using System;

namespace Cloudents.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public sealed class PublicValueAttribute : Attribute
    {
    }
}
