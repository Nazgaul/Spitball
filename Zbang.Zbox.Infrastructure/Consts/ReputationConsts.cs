
using System.Reflection;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Infrastructure.Consts
{
    public static class ReputationConst
    {
        public const int ItemLike = 15;
        public const double ItemView = 0.1;
        public const double ItemDownload = 0.2;

        public const int QuizView = 5;
        public const int QuizSolve = 25;
        public const int QuizLike = 25;

        public const int FlashcardView = 5;
        public const int FlashcardLike = 25;

        public const int CommentLike = 5;
        public const int ReplyLike = 5;

        public const int BadgeRegister = 500;
        public const int BadgeFollowClass = 500;
        public const int BadgeCreateQuizzes = 500;
        public const int BadgeUploadFiles = 500;
        public const int BadgeLikes = 1500;

        public static int GetBadgeScore(BadgeType badge)
        {
            foreach (var fi in typeof(ReputationConst).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (fi.Name == "Badge" + badge)
                {
                    return (int) fi.GetValue(null);
                }
            }
            return 0;
        }
    }
}
