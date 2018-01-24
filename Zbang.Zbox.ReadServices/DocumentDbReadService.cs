using System.Threading.Tasks;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.ReadServices
{
    public class DocumentDbReadService : IDocumentDbReadService
    {
        private readonly IDocumentDbRepository<Flashcard> _flashcardRepository;

        public DocumentDbReadService(IDocumentDbRepository<Flashcard> flashcardRepository)
        {
            _flashcardRepository = flashcardRepository;
        }

        public Task<Flashcard> FlashcardAsync(long id)
        {
            return _flashcardRepository.GetItemAsync(id.ToString());
        }

        //public Task<IEnumerable<Flashcard>> FavoriteFlashcardsAsync(IEnumerable<long> ids)
        //{
        //    var idsStr = ids.Select(s => $"\"{s}\"");
        //    return m_FlashcardRepository.GetItemsAsync($"SELECT * FROM c where c.id in ({string.Join(",", idsStr)})");
        //}
    }
}
