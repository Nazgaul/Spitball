using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateFlashcardCommandHandler : ICommandHandlerAsync<UpdateFlashcardCommand>
    {
        private readonly IDocumentDbRepository<Flashcard> m_FlashcardRepository;
        private readonly IRepository<FlashCardMeta> m_FlashcardMetaRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;

        public UpdateFlashcardCommandHandler(IDocumentDbRepository<Flashcard> flashcardRepository, IRepository<FlashCardMeta> flashcardMetaRepository, IUserRepository userRepository, IBoxRepository boxRepository)
        {
            m_FlashcardRepository = flashcardRepository;
            m_FlashcardMetaRepository = flashcardMetaRepository;
            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
        }

        public async Task HandleAsync(UpdateFlashcardCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = m_UserRepository.Load(message.Flashcard.UserId);
            var box = m_BoxRepository.Load(message.Flashcard.BoxId);
            var flashcard = m_FlashcardMetaRepository.Load(message.Flashcard.Id);
            if (flashcard.User != user)
            {
                throw new UnauthorizedAccessException();
            }
            if (flashcard.Box != box)
            {
                throw new UnauthorizedAccessException();
            }
            flashcard.Name = message.Flashcard.Name;
            flashcard.DateTimeUser.UpdateUserTime(message.Flashcard.Id);
            m_FlashcardMetaRepository.Save(flashcard);
            await m_FlashcardRepository.UpdateItemAsync(message.Flashcard.id, message.Flashcard);
        }
    }
}