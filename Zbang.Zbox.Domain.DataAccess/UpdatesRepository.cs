using System;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class UpdatesRepository : NHibernateRepository<Updates>, IUpdatesRepository
    {
        public void DeleteUserUpdateByBoxId(long userId, long boxId)
        {
            var query = UnitOfWork.CurrentSession.GetNamedQuery("DeleteUserUpdatesByBoxId");
            query.SetInt64("userid", userId);
            query.SetInt64("boxId", boxId);
            query.ExecuteUpdate();

        }

      

        public void DeleteReplyUpdates(Guid answerId)
        {
            var query = UnitOfWork.CurrentSession.GetNamedQuery("DeleteUpdatesByAnswerId");
            query.SetGuid("ReplyId", answerId);
            query.ExecuteUpdate();
        }
        public void DeleteCommentUpdates(Guid commentId)
        {
            var query = UnitOfWork.CurrentSession.GetNamedQuery("DeleteUpdatesByCommentId");
            query.SetGuid("CommentId", commentId);
            query.ExecuteUpdate();
        }

        public void DeleteQuizUpdates(long quizId)
        {
            var query = UnitOfWork.CurrentSession.GetNamedQuery("DeleteUpdatesByQuizId");
            query.SetInt64("QuizId", quizId);
            query.ExecuteUpdate();
        }
        public void DeleteQuizDiscussionUpdates(Guid quizDiscussionId, long quizId)
        {
            var query = UnitOfWork.CurrentSession.GetNamedQuery("DeleteUpdatesByQuizDiscussionId");
            query.SetGuid("QuizDiscussionId", quizDiscussionId);
            query.SetInt64("QuizId", quizId); //index wise
            query.ExecuteUpdate();
        }

        public void DeleteUserItemUpdateByBoxId(long userId, long boxId)
        {
            var query = UnitOfWork.CurrentSession.GetNamedQuery("DeleteItemUpdatesByBoxIdUserId");
            query.SetInt64("userid", userId);
            query.SetInt64("boxId", boxId);
            query.ExecuteUpdate();
        }


    }
}
