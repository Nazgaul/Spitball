using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IItemRepository : IRepository<Item>
    {
        bool CheckFileNameExists(string fileName, Box box);
        Comment GetPreviousCommentId(Box box, User user);
    }
}