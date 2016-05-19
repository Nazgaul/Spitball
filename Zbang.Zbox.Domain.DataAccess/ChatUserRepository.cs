using System;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class ChatUserRepository : NHibernateRepository<ChatUser>, IChatUserRepository
    {
        public void DeleteUserUpdateByFeedId(long userId,  Guid chatRoomId)
        {
            var query = UnitOfWork.CurrentSession.GetNamedQuery("DeleteUnReadChatMessages");
            query.SetInt64("userid", userId);
            query.SetGuid("chatRoom", chatRoomId);
            query.ExecuteUpdate();

        }
    }

    public interface IChatUserRepository : IRepository<ChatUser>
    {
        void DeleteUserUpdateByFeedId(long userId, Guid chatRoomId);
    }
}
