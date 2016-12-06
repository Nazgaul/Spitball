using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Enums
{
    // ReSharper disable once EnumUnderlyingTypeIsInt - need for dapper
    public enum BadgeType : int
    {
        None,
        [ResourceDescription(typeof(Gamification), "BadgeRegister")]

        Register,
        [ResourceDescription(typeof(Gamification), "BadgeFollowClass")]
        FollowClass,
        [ResourceDescription(typeof(Gamification), "BadgeCreateQuizzes")]
        CreateQuizzes,
        [ResourceDescription(typeof(Gamification), "BadgeUploadFiles")]
        UploadFiles,
        [ResourceDescription(typeof(Gamification), "BadgeLikes")]
        Likes
    }
}