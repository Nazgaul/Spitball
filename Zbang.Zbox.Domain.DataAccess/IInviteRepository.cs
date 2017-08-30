using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IInviteRepository : IRepository<Invite>
    {
        // InviteToBox GetUserInvite(long userBoxRelId);
        // IEnumerable<InviteToBox> GetUserInvites(long recipientId);
        IEnumerable<Invite> GetUserInvitesToBox(string email, long boxId);
        IEnumerable<Invite> GetUserInvites(string email, Guid id);
    }
}
