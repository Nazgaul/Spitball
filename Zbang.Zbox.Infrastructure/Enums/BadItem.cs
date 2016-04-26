using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Enums.Resources;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum BadItem
    {
        [EnumDescriptionAttribute(typeof(EnumResources), "Infringe")]
        RightViolation = 1,
        [EnumDescriptionAttribute(typeof(EnumResources), "Spam")]
        Spam,
        [EnumDescriptionAttribute(typeof(EnumResources), "Nudity")]
        Porn,
        [EnumDescriptionAttribute(typeof(EnumResources), "Attacks")]
        Attack,
        [EnumDescriptionAttribute(typeof(EnumResources), "Hateful")]
        Hate,
        [EnumDescriptionAttribute(typeof(EnumResources), "Other")]
        Other
    }
}
