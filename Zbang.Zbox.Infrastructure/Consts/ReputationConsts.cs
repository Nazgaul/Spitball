

namespace Zbang.Zbox.Infrastructure.Consts
{
    public static class ReputationConsts
    {
        public const int AddQuizScore = 300;
        public const int UploadItemScore = 100;
        public const int AddAnswerScore = 100;
        public const int AddQuestionScore = 50;
        public const int DeleteItemScore = -UploadItemScore;
        public const int DeleteQuestionScore = -AddQuestionScore;
        public const int DeleteAnswerScore = -AddAnswerScore;
        public const int DeleteQuizScore = -AddQuizScore;
        public const int ShareFabookScore = 50;
        public const int InviteToCloudentsScore = 150;
        public const int InviteToBoxScore = 50;
        public const int Register = 500;

        public const int Rate3StareScore = 50;
        public const int Rate4StareScore = 100;
        public const int Rate5StareScore = 150;

        public const int UnRate3StareScore = -Rate3StareScore;
        public const int UnRate4StareScore = -Rate4StareScore;
        public const int UnRate5StareScore = -Rate5StareScore;

        public const int AddItemCommentScore = 30;
        public const int AddItemReplyScore = 15;

        public const int DeleteItemCommentScore = -AddItemCommentScore;
        public const int DeleteItemReplyScore = -AddItemReplyScore;
    }
}
