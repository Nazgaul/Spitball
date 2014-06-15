using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IItemRateRepository : IRepository<ItemRate>
    {
        int GetRateCount(long itemId);
        ItemRate GetRateOfUser(long userId, long itemId);
    }
}
