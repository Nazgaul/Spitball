using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IUpdatesRepository: IRepository<Updates>
    {
        //IEnumerable<Updates> GetUserBoxUpdates(long userId, long boxId);
        void DeleteUserUpdateByBoxId(long userId, long boxId);
        void DeleteUserUpdateByFeedId(long userId, long boxId, Guid commentId);
        void DeleteUserItemUpdateByBoxId(long userId, long boxId);
        void DeleteReplyUpdatesByBoxId(long boxId, Guid answerId);
    }
}
