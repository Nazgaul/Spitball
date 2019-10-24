using Cloudents.Core.Attributes;
using Cloudents.Core.Enum.Resources;

namespace Cloudents.Core.Enum
{
    public enum TransactionType
    {
        [ResourceDescription(typeof(EnumResources), "TransactionTypeEarned")]
        Earned,
        [ResourceDescription(typeof(EnumResources), "TransactionTypeStake")]
        Stake,
        [ResourceDescription(typeof(EnumResources), "TransactionTypeSpent")]
        Spent
    }
}