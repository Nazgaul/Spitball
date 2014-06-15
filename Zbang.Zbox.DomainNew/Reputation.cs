using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    public class Reputation
    {
        protected Reputation()
        {

        }
        public Reputation(Guid id, User user, ReputationAction action)
        {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = id;
            User = user;
            CreationTime = DateTime.UtcNow;
            Action = action;
            Score = CalcuateScore(action);
// ReSharper restore DoNotCallOverridableMethodsInConstructor

        }
        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }
        public virtual DateTime CreationTime { get; set; }
        public virtual int Score { get; set; }
        public virtual ReputationAction Action { get; set; }

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

        public const int Rate3StareScore = 5;
        public const int Rate4StareScore = 10;
        public const int Rate5StareScore = 15;

        public const int UnRate3StareScore = -Rate3StareScore;
        public const int UnRate4StareScore = -Rate4StareScore;
        public const int UnRate5StareScore = -Rate5StareScore;




        internal static int CalcuateScore(ReputationAction action)
        {
            switch (action)
            {
                case ReputationAction.None:
                    return 0;
                case ReputationAction.AddItem:
                    return UploadItemScore;
                case ReputationAction.AddAnswer:
                    return AddAnswerScore;
                case ReputationAction.AddQuestion:
                    return AddQuestionScore;
                case ReputationAction.DeleteItem:
                    return DeleteItemScore;
                case ReputationAction.DeleteQuestion:
                    return DeleteQuestionScore;
                case ReputationAction.DeleteAnswer:
                    return DeleteAnswerScore;
                case ReputationAction.ShareFacebook:
                    return ShareFabookScore;
                case ReputationAction.Invite:
                    return InviteToCloudentsScore;
                case ReputationAction.InviteToBox:
                    return InviteToBoxScore;
                case ReputationAction.Rate3Stars:
                    return Rate3StareScore;
                case ReputationAction.Rate4Stars:
                    return Rate4StareScore;
                case ReputationAction.Rate5Stars:
                    return Rate5StareScore;
                case ReputationAction.UnRate3Stars:
                    return UnRate3StareScore;
                case ReputationAction.UnRate4Stars:
                    return UnRate4StareScore;
                case ReputationAction.UnRate5Stars:
                    return UnRate5StareScore;
                case ReputationAction.AddQuiz:
                    return AddQuizScore;
                case ReputationAction.DelteQuiz:
                    return DeleteQuizScore;
                default:
                    return 0;
            }
        }

    }
}
