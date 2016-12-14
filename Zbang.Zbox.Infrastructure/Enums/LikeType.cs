using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum LikeType
    {
        [ResourceDescription(typeof(EnumResources), "LikeTypeComment")]
        Comment,
        [ResourceDescription(typeof(EnumResources), "LikeTypeReply")]
        Reply,
        [ResourceDescription(typeof(EnumResources), "LikeTypeItem")]
        Item
    }

    public enum SeoType
    {
        Static,
        Course,
        Item,
        Quiz,
        Flashcard
    }
}
