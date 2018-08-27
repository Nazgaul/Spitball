using Cloudents.Core.Attributes;

namespace Cloudents.Core.Enum
{
    public enum TransactionType
    {
        //None,
        [PublicValue]
        Awarded,
        [PublicValue]
        Earned,
        Stake,
        Spent
    }
}