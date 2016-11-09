using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Infrastructure.Repositories;

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
    }
}
