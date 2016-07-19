using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Enums.Resources;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum BadItem
    {
        [EnumDescription(typeof(EnumResources), "Infringe")]
        RightViolation = 1,
        [EnumDescription(typeof(EnumResources), "Spam")]
        Spam,
        [EnumDescription(typeof(EnumResources), "Nudity")]
        Porn,
        [EnumDescription(typeof(EnumResources), "Attacks")]
        Attack,
        [EnumDescription(typeof(EnumResources), "Hateful")]
        Hate,
        [EnumDescription(typeof(EnumResources), "Other")]
        Other
    }
}
