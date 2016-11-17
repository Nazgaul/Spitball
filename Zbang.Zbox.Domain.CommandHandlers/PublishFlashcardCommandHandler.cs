using System;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class PublishFlashcardCommandHandler : ICommandHandlerAsync<PublishFlashcardCommand>
    {
        private readonly IDocumentDbRepository<Flashcard> m_FlashcardRepository;
        private readonly IRepository<FlashcardMeta> m_FlashcardMetaRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;

        public PublishFlashcardCommandHandler(IDocumentDbRepository<Flashcard> flashcardRepository, IRepository<FlashcardMeta> flashcardMetaRepository, IUserRepository userRepository, IBoxRepository boxRepository)
        {
            m_FlashcardRepository = flashcardRepository;
            m_FlashcardMetaRepository = flashcardMetaRepository;
            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
        }

        public async Task HandleAsync(PublishFlashcardCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var box = m_BoxRepository.Load(message.Flashcard.BoxId);
            var flashCardWithSameName = box.Flashcards.FirstOrDefault(w => w.Name == message.Flashcard.Name && w.Publish && w.Id != message.Flashcard.Id);
            if (flashCardWithSameName != null)
            {
                throw new ArgumentException("same flashcard name");
            }
            var user = m_UserRepository.Load(message.Flashcard.UserId);
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
            if (message.Flashcard.Cards != null)
            {
                flashcard.CardCount = message.Flashcard.Cards.Count();
            }
            flashcard.DateTimeUser.UpdateUserTime(message.Flashcard.Id);
            flashcard.Pins?.Clear();
            flashcard.Publish = true;
            m_FlashcardMetaRepository.Save(flashcard);
            box.UpdateFlashcardCount();
            m_BoxRepository.Save(box);
            await m_FlashcardRepository.UpdateItemAsync(message.Flashcard.id, message.Flashcard);
        }
    }
}