using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Infrastructure.Repositories;
using System.Linq;

namespace Zbang.Zbox.ReadServices
{
    public class DocumentDbReadService : IDocumentDbReadService
    {
        private readonly IDocumentDbRepository<Flashcard> m_FlashcardRepository;

        public DocumentDbReadService(IDocumentDbRepository<Flashcard> flashcardRepository)
        {
            m_FlashcardRepository = flashcardRepository;
        }

        public Task<Flashcard> FlashcardAsync(long id)
        {
            return m_FlashcardRepository.GetItemAsync(id.ToString());
        }

        public Task<IEnumerable<Flashcard>> FavoriteFlashcardsAsync(IEnumerable<long> ids)
        {
            var idsStr = ids.Select(s => $"\"{s}\"");
            return m_FlashcardRepository.GetItemsAsync($"SELECT * FROM c where c.id in ({string.Join(",", idsStr)})");
        }
    }
}
