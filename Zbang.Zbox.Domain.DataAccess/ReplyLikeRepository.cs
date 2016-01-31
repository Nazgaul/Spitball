using System;
using System.Linq;
using NHibernate.Linq;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class ReplyLikeRepository : NHibernateRepository<ReplyLike>, IReplyLikeRepository
    {
        public ReplyLike GetUserLike(long userId, Guid replyId)
        {
            return UnitOfWork.CurrentSession.Query<ReplyLike>()
                .FirstOrDefault(w => w.Reply.Id == replyId && w.User.Id == userId);
        }
    }
}