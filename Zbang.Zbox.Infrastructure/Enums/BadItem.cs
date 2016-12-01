using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Enums.Resources;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum BadItem
    {
        [ResourceDescription(typeof(EnumResources), "Infringe")]
        RightViolation = 1,
        [ResourceDescription(typeof(EnumResources), "Spam")]
        Spam,
        [ResourceDescription(typeof(EnumResources), "Nudity")]
        Porn,
        [ResourceDescription(typeof(EnumResources), "Attacks")]
        Attack,
        [ResourceDescription(typeof(EnumResources), "Hateful")]
        Hate,
        [ResourceDescription(typeof(EnumResources), "Other")]
        Other
    }
}
