using Cloudents.Common.Attributes;
using Cloudents.Common.Resources;

namespace Cloudents.Common.Enum
{
    public enum TransactionType
    {
        //None,
        //[PublicValue]
        //[ResourceDescription(typeof(EnumResources), "TransactionTypeAwarded")]
        //Awarded,
        [ResourceDescription(typeof(EnumResources), "TransactionTypeEarned")]
        Earned,
        [ResourceDescription(typeof(EnumResources), "TransactionTypeStake")]
        Stake,
        [ResourceDescription(typeof(EnumResources), "TransactionTypeSpent")]
        Spent
    }
}