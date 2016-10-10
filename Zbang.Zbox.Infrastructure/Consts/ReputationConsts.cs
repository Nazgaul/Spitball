

namespace Zbang.Zbox.Infrastructure.Consts
{
    public static class ReputationConst
    {
        public const int AddQuizScore = 50;
        public const int UploadItemScore = 100;
        public const int AddAnswerScore = 100;
        public const int AddQuestionScore = 50;
        public const int DeleteItemScore = -UploadItemScore;
        public const int DeleteQuestionScore = -AddQuestionScore;
        public const int DeleteAnswerScore = -AddAnswerScore;
        public const int DeleteQuizScore = -AddQuizScore;
        public const int ShareFacebookScore = 50;
        public const int InviteToSystemScore = 150;
        public const int InviteToBoxScore = 50;
        public const int Register = 500;

        //public const int Rate3StarScore = 50;
        //public const int Rate4StarScore = 100;
        public const int LikeItem = 150;

        //public const int Unrate3StarScore = -Rate3StarScore;
        //public const int Unrate4StarScore = -Rate4StarScore;
        public const int DislikeItem = -LikeItem;

        //public const int AddItemCommentScore = 30;
        //public const int AddItemReplyScore = 15;

        //public const int DeleteItemCommentScore = -AddItemCommentScore;
        //public const int DeleteItemReplyScore = -AddItemReplyScore;
    }
}
