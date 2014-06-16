using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IInviteToCloudentsRepository: IRepository<InviteToCloudents>
    {
        InviteToCloudents GetInviteToCloudents(User recepient);
    }
}
