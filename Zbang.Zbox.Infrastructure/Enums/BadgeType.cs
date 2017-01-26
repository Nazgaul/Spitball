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

    [Flags]
    public enum ItemType : int
    {
        Undefined = 0,
        Flashcard = 1,
        Quiz = 2,
        Document = 4,
        Link = 8,
        Homework = 16,
        Lecture = 32,
        StudyGuide = 64,
        Exam = 128,
        ClassNote = 256
    }

  

}