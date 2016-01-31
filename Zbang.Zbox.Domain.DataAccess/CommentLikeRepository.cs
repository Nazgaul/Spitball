using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class CommentLikeRepository : NHibernateRepository<CommentLike>, ICommentLikeRepository
    {
        public CommentLike GetUserLike(long userId, Guid commentId)
        {
            return UnitOfWork.CurrentSession.Query<CommentLike>()
                .FirstOrDefault(w => w.Comment.Id == commentId && w.User.Id == userId);
        }
    }
}
