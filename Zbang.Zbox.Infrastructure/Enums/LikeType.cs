using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum LikeType
    {
        [EnumDescriptionAttribute(typeof(EnumResources), "LikeTypeComment")]
        Comment,
        [EnumDescriptionAttribute(typeof(EnumResources), "LikeTypeReply")]
        Reply,
        [EnumDescriptionAttribute(typeof(EnumResources), "LikeTypeItem")]
        Item
    }
}
