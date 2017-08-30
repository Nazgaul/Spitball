using System;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IUpdatesRepository: IRepository<Updates>
    {
        void DeleteUserUpdateByBoxId(long userId, long boxId);
       // void DeleteUserUpdateByFeedId(long userId, long boxId, Guid commentId);
        void DeleteUserItemUpdateByBoxId(long userId, long boxId);
        void DeleteReplyUpdates( Guid answerId);
        void DeleteCommentUpdates(Guid commentId);

        void DeleteQuizUpdates(long quizId);
        void DeleteQuizDiscussionUpdates(Guid quizDiscussionId, long quizId);
    }
}
