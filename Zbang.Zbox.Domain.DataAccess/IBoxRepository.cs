using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IBoxRepository : IRepository<Box>
    {
        Box GetBoxWithSameName(string name, User user);
        int QnACount(long id);
    }
}
