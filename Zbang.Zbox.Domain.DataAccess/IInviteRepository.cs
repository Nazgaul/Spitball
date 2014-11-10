using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IInviteRepository : IRepository<Invite>
    {
        InviteToBox GetUserInvite(UserBoxRel userBoxRel);
        IEnumerable<InviteToBox> GetUserInvites(long recipientId);
    }
}
