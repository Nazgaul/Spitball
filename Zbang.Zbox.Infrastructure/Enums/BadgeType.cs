namespace Zbang.Zbox.Infrastructure.Enums
{
    // ReSharper disable once EnumUnderlyingTypeIsInt - need for dapper
    public enum BadgeType : int
    {
        None,
        Register,
        FollowClass,
        CreateQuizzes,
        UploadFiles,
        Likes
    }
}