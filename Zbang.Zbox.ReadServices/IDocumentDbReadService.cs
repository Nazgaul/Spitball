using System.Threading.Tasks;
using Zbang.Zbox.Domain;

namespace Zbang.Zbox.ReadServices
{
    public interface IDocumentDbReadService
    {
        Task<Flashcard> FlashcardAsync(long id);
       // Task<IEnumerable<Flashcard>> FavoriteFlashcardsAsync(IEnumerable<long> ids);
    }
}