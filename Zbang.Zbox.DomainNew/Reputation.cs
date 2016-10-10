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
                    return ReputationConst.UploadItemScore;
                case ReputationAction.AddReply:
                    return ReputationConst.AddAnswerScore;
                case ReputationAction.AddComment:
                    return ReputationConst.AddQuestionScore;
                case ReputationAction.DeleteItem:
                    return ReputationConst.DeleteItemScore;
                case ReputationAction.DeleteComment:
                    return ReputationConst.DeleteQuestionScore;
                case ReputationAction.DeleteReply:
                    return ReputationConst.DeleteAnswerScore;
                case ReputationAction.ShareFacebook:
                    return ReputationConst.ShareFacebookScore;
                case ReputationAction.Invite:
                    return ReputationConst.InviteToSystemScore;
                case ReputationAction.InviteToBox:
                    return ReputationConst.InviteToBoxScore;
                case ReputationAction.LikeDocument:
                    return ReputationConst.LikeItem;
                case ReputationAction.UnlikeDocument:
                    return ReputationConst.DislikeItem;
                //case ReputationAction.LikeDocument:
                //    return ReputationConst.LikeItem;
                //case ReputationAction.UnRate3Stars:
                //    return ReputationConst.Unrate3StarScore;
                //case ReputationAction.UnRate4Stars:
                //    return ReputationConst.Unrate4StarScore;
                //case ReputationAction.UnLikeDocument:
                //    return ReputationConst.DisLikeItem;
                case ReputationAction.AddQuiz:
                    return ReputationConst.AddQuizScore;
                case ReputationAction.DeleteQuiz:
                    return ReputationConst.DeleteQuizScore;
                case ReputationAction.Register:
                    return ReputationConst.Register;
                //case ReputationAction.AddItemComment:
                //    return ReputationConsts.AddItemCommentScore;
                //case ReputationAction.AddItemReply:
                //    return ReputationConsts.AddItemReplyScore;
                //case ReputationAction.DeleteItemComment:
                //    return ReputationConsts.DeleteItemCommentScore;
                //case ReputationAction.DeleteItemReply:
                //    return ReputationConsts.DeleteItemReplyScore;
                default:
                    return 0;
            }
        }

    }
}
