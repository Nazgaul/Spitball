using System;

namespace Cloudents.Core.Enum
{
    [Serializable]
    public enum ItemState
    {
        Ok,
        Deleted,
        Pending,
        Flagged
    }

    public enum PriceType
    {
        Free,
        HasPrice,
        Subscriber
    }
}