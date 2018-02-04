using System.Threading.Tasks;
using Zbang.Zbox.Domain;

namespace Zbang.Zbox.ReadServices
{
    public interface IDocumentDbReadService
    {
        Task<Flashcard> FlashcardAsync(long id);
    }
}