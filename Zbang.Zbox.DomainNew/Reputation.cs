using System;
using Zbang.Zbox.Infrastructure.Consts;
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
            Score = CalculateScore(action);
// ReSharper restore DoNotCallOverridableMethodsInConstructor

        }
        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }
        public virtual DateTime CreationTime { get; set; }
        public virtual int Score { get; set; }
        public virtual ReputationAction Action { get; set; }

       




        internal static int CalculateScore(ReputationAction action)
        {
            switch (action)
            {
                case ReputationAction.None:
                    return 0;
                case ReputationAction.AddItem:
                    return ReputationConsts.UploadItemScore;
                case ReputationAction.AddAnswer:
                    return ReputationConsts.AddAnswerScore;
                case ReputationAction.AddQuestion:
                    return ReputationConsts.AddQuestionScore;
                case ReputationAction.DeleteItem:
                    return ReputationConsts.DeleteItemScore;
                case ReputationAction.DeleteQuestion:
                    return ReputationConsts.DeleteQuestionScore;
                case ReputationAction.DeleteAnswer:
                    return ReputationConsts.DeleteAnswerScore;
                case ReputationAction.ShareFacebook:
                    return ReputationConsts.ShareFabookScore;
                case ReputationAction.Invite:
                    return ReputationConsts.InviteToCloudentsScore;
                case ReputationAction.InviteToBox:
                    return ReputationConsts.InviteToBoxScore;
                case ReputationAction.Rate3Stars:
                    return ReputationConsts.Rate3StareScore;
                case ReputationAction.Rate4Stars:
                    return ReputationConsts.Rate4StareScore;
                case ReputationAction.Rate5Stars:
                    return ReputationConsts.Rate5StareScore;
                case ReputationAction.UnRate3Stars:
                    return ReputationConsts.UnRate3StareScore;
                case ReputationAction.UnRate4Stars:
                    return ReputationConsts.UnRate4StareScore;
                case ReputationAction.UnRate5Stars:
                    return ReputationConsts.UnRate5StareScore;
                case ReputationAction.AddQuiz:
                    return ReputationConsts.AddQuizScore;
                case ReputationAction.DeleteQuiz:
                    return ReputationConsts.DeleteQuizScore;
                case ReputationAction.Register:
                    return ReputationConsts.Register;
                case ReputationAction.AddItemComment:
                    return ReputationConsts.AddItemCommentScore;
                case ReputationAction.AddItemReply:
                    return ReputationConsts.AddItemReplyScore;
                case ReputationAction.DeleteItemComment:
                    return ReputationConsts.DeleteItemCommentScore;
                case ReputationAction.DeleteItemReply:
                    return ReputationConsts.DeleteItemReplyScore;
                default:
                    return 0;
            }
        }

    }
}
