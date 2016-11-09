using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddFlashcardCommandHandler : ICommandHandlerAsync<AddFlashcardCommand>
    {
        private readonly IDocumentDbRepository<Flashcard> m_FlashcardRepository;
        private readonly IRepository<FlashCardMeta> m_FlashcardMetaRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;

        public AddFlashcardCommandHandler(IDocumentDbRepository<Flashcard> flashcardRepository, IRepository<FlashCardMeta> flashcardMetaRepository, IUserRepository userRepository, IBoxRepository boxRepository)
        {
            m_FlashcardRepository = flashcardRepository;
            m_FlashcardMetaRepository = flashcardMetaRepository;
            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
        }

        public async Task HandleAsync(AddFlashcardCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = m_UserRepository.Load(message.Flashcard.UserId);
            var box = m_BoxRepository.Load(message.Flashcard.BoxId);
            var flashcard = new FlashCardMeta(message.Flashcard.Id, message.Flashcard.Name, user, box);
            m_FlashcardMetaRepository.Save(flashcard);
            await m_FlashcardRepository.CreateItemAsync(message.Flashcard);
        }
    }
}
