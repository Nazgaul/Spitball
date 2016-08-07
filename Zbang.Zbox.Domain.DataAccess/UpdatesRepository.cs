﻿using System;
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

        public void DeleteUserUpdateByFeedId(long userId, long boxId, Guid commentId)
        {
            var query = UnitOfWork.CurrentSession.GetNamedQuery("DeleteUpdatesByFeedId");
            query.SetInt64("userid", userId);
            query.SetInt64("boxId", boxId);
            query.SetGuid("CommentId", commentId);
            query.ExecuteUpdate();
        }

        public void DeleteReplyUpdatesByBoxId(long boxId, Guid answerId)
        {
            var query = UnitOfWork.CurrentSession.GetNamedQuery("DeleteUpdatesByAnswerId");
            query.SetInt64("boxId", boxId);
            query.SetGuid("ReplyId", answerId);
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
