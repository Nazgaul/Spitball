using System;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface ICommentLikeRepository : IRepository<CommentLike>
    {
        CommentLike GetUserLike(long userId, Guid commentId);
    }
}
