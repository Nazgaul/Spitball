using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteFlashcardCommandHandler : ICommandHandler<DeleteFlashcardCommand>
    {
        private readonly IRepository<FlashCardMeta> m_FlashcardMetaRepository;
        private readonly IUserRepository m_UserRepository;

        public DeleteFlashcardCommandHandler(IRepository<FlashCardMeta> flashcardMetaRepository,
            IUserRepository userRepository)
        {
            m_FlashcardMetaRepository = flashcardMetaRepository;
            m_UserRepository = userRepository;
        }

        public void Handle(DeleteFlashcardCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = m_UserRepository.Load(message.UserId);
            var flashcard = m_FlashcardMetaRepository.Load(message.Id);
            if (flashcard.User != user)
            {
                throw new UnauthorizedAccessException();
            }

            m_FlashcardMetaRepository.Delete(flashcard);
        }
    }
}