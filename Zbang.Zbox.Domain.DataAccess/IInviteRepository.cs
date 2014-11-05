using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IInviteRepository : IRepository<Invite>
    {
        InviteToBox GetUserInvite(UserBoxRel userBoxRel);
    }
}
