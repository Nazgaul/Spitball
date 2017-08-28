using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddFlashcardPinCommandHandler : ICommandHandler<AddFlashcardPinCommand>
    {
        private readonly IRepository<FlashcardPin> m_FlashcardPinRepository;
        private readonly IRepository<FlashcardMeta> m_FlashcardRepository;
        private readonly IGuidIdGenerator m_GuidGenerator;
        private readonly IUserRepository m_UserRepository;

        public AddFlashcardPinCommandHandler(IRepository<FlashcardPin> flashcardPinRepository, IRepository<FlashcardMeta> flashcardRepository, IGuidIdGenerator guidGenerator, IUserRepository userRepository)
        {
            m_FlashcardPinRepository = flashcardPinRepository;
            m_FlashcardRepository = flashcardRepository;
            m_GuidGenerator = guidGenerator;
            m_UserRepository = userRepository;
        }

        public void Handle(AddFlashcardPinCommand message)
        {
            var user = m_UserRepository.Load(message.UserId);
            var flashcard = m_FlashcardRepository.Load(message.FlashCardId);
            var pin = new FlashcardPin(m_GuidGenerator.GetId(), user, flashcard, message.Index);
            m_FlashcardPinRepository.Save(pin);
        }
    }
}
