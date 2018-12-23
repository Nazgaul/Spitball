using System;

namespace Cloudents.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public sealed class OrderValueAttribute : Attribute
    {
        public OrderValueAttribute(int order)
        {
            Order = order;
        }

        public int Order { get;  }
    }
}