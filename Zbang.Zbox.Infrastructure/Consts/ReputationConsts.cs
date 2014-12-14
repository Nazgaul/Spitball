

namespace Zbang.Zbox.Infrastructure.Consts
{
    public static class ReputationConsts
    {
        public const int AddQuizScore = 30;
        public const int UploadItemScore = 10;
        public const int AddAnswerScore = 10;
        public const int AddQuestionScore = 5;
        public const int DeleteItemScore = -UploadItemScore;
        public const int DeleteQuestionScore = -AddQuestionScore;
        public const int DeleteAnswerScore = -AddAnswerScore;
        public const int DeleteQuizScore = -AddQuizScore;
        public const int ShareFabookScore = 5;
        public const int InviteToCloudentsScore = 20;
        public const int InviteToBoxScore = 5;
        public const int Register = 500;

        public const int Rate3StareScore = 5;
        public const int Rate4StareScore = 10;
        public const int Rate5StareScore = 15;

        public const int UnRate3StareScore = -Rate3StareScore;
        public const int UnRate4StareScore = -Rate4StareScore;
        public const int UnRate5StareScore = -Rate5StareScore;
    }
}
