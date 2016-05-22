using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class ChatUserRepository : NHibernateRepository<ChatUser>, IChatUserRepository
    {
        public void DeleteUserUpdateByFeedId(long userId, Guid chatRoomId)
        {
            var query = UnitOfWork.CurrentSession.GetNamedQuery("DeleteUnReadChatMessages");
            query.SetInt64("userid", userId);
            query.SetGuid("chatRoom", chatRoomId);
            query.ExecuteUpdate();

        }

        public Guid? GetChatRoom(IEnumerable<long> userIds)
        {
            var ids = userIds.ToList();
            var query = UnitOfWork.CurrentSession.GetNamedQuery("GetChatRoom");
            query.SetInt32("length", ids.Count);
            query.SetParameterList("userids", ids);
            return query.UniqueResult<Guid?>();
        }
    }

    public interface IChatUserRepository : IRepository<ChatUser>
    {
        void DeleteUserUpdateByFeedId(long userId, Guid chatRoomId);
        Guid? GetChatRoom(IEnumerable<long> userIds);
    }
}
