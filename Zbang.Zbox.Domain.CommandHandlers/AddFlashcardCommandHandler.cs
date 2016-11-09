using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddFlashcardCommandHandler : ICommandHandlerAsync<AddFlashcardCommand>
    {
        private readonly IDocumentDbRepository<Flashcard> m_FlashcardRepository;

        public AddFlashcardCommandHandler(IDocumentDbRepository<Flashcard> flashcardRepository)
        {
            m_FlashcardRepository = flashcardRepository;
        }

        public Task HandleAsync(AddFlashcardCommand message)
        {
            return m_FlashcardRepository.CreateItemAsync(message.Flashcard);
        }
    }
}
