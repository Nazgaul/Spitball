using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IItemTabRepository : IRepository<ItemTab>
    {
        ItemTab GetTabWithTheSameName(string name, long boxId);
    }
}
