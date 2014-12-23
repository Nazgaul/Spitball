using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IItemRepository : IRepository<Item>
    {
        bool CheckFileNameExists(string fileName, long boxId);
        Comment GetPreviousCommentId(long boxId, long userId);
    }
}