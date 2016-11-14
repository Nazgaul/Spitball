using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteFlashcardCommandHandler : ICommandHandlerAsync<DeleteFlashcardCommand>
    {
        private readonly IRepository<FlashcardMeta> m_FlashcardMetaRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IDocumentDbRepository<Flashcard> m_FlashcardRepository;

        public DeleteFlashcardCommandHandler(IRepository<FlashcardMeta> flashcardMetaRepository,
            IUserRepository userRepository, IDocumentDbRepository<Flashcard> flashcardRepository)
        {
            m_FlashcardMetaRepository = flashcardMetaRepository;
            m_UserRepository = userRepository;
            m_FlashcardRepository = flashcardRepository;
        }

        public async Task HandleAsync(DeleteFlashcardCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = m_UserRepository.Load(message.UserId);
            var flashcardMeta = m_FlashcardMetaRepository.Load(message.Id);
            if (flashcardMeta.User != user)
            {
                throw new UnauthorizedAccessException();
            }
            var flashcard =  await m_FlashcardRepository.GetItemAsync(message.Id.ToString());
            flashcard.IsDeleted = true;
            await m_FlashcardRepository.UpdateItemAsync(message.Id.ToString(), flashcard);
            m_FlashcardMetaRepository.Delete(flashcardMeta);
        }
    }
}