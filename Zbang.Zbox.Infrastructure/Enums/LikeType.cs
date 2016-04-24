using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum LikeType
    {
        [EnumDescription(typeof(EnumResources), "LikeTypeComment")]
        Comment,
        [EnumDescription(typeof(EnumResources), "LikeTypeReply")]
        Reply,
        [EnumDescription(typeof(EnumResources), "LikeTypeItem")]
        Item
    }
}
