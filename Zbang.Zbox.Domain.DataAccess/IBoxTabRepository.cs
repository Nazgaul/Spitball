using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IBoxTabRepository : IRepository<BoxTab>
    {
        BoxTab GetTabWithTheSameName(string name, long userId);
    }
}
