using System.Linq;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class FlashcardPinRepository : NHibernateRepository<FlashcardPin>, IFlashcardPinRepository
    {
        public FlashcardPin GetUserPin(long userId, long id, int index)
        {
            return UnitOfWork.CurrentSession.Query<FlashcardPin>()
                .FirstOrDefault(w => w.Flashcard.Id == id && w.User.Id == userId && w.Index == index);
        }
    }
}