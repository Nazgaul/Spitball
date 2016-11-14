using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IFlashcardPinRepository : IRepository<FlashcardPin>
    {
        FlashcardPin GetUserPin(long userId, long id, int index);
    }
}