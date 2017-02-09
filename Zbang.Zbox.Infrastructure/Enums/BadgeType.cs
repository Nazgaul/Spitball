using System;
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

    public enum ItemType : int
    {
        Undefined = 0,
        Flashcard = 1,
        Quiz = 2,
        Document, // not useed
        Link, // not used
        Homework = 5,
        Lecture = 6,
        StudyGuide = 7,
        Exam,
        ClassNote
    }

    public enum SearchType
    {
        None,
        Document,
        Member,
        Feed
    }

    public enum TagType
    {
        None,
        Watson,
        User,
        Backoffice
    }



}