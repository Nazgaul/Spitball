using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteFlashcardCommandHandler : ICommandHandlerAsync<DeleteFlashcardCommand>
    {
        private readonly IRepository<FlashcardMeta> m_FlashcardMetaRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IDocumentDbRepository<Flashcard> m_FlashcardRepository;
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IQueueProvider m_QueueProvider;

        public DeleteFlashcardCommandHandler(IRepository<FlashcardMeta> flashcardMetaRepository,
            IUserRepository userRepository, IDocumentDbRepository<Flashcard> flashcardRepository, IRepository<Box> boxRepository, IQueueProvider queueProvider)
        {
            m_FlashcardMetaRepository = flashcardMetaRepository;
            m_UserRepository = userRepository;
            m_FlashcardRepository = flashcardRepository;
            m_BoxRepository = boxRepository;
            m_QueueProvider = queueProvider;
        }

        public async Task HandleAsync(DeleteFlashcardCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var user = m_UserRepository.Load(message.UserId);
            var flashcardMeta = m_FlashcardMetaRepository.Load(message.Id);

            bool isAuthorize = flashcardMeta.User.Id == message.UserId
               || flashcardMeta.Box.Owner.Id == message.UserId
               || user.IsAdmin();

            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException();
            }


            //if (flashcardMeta.User != user)
            //{
            //    throw new UnauthorizedAccessException();
            //}
            var flashcard =  await m_FlashcardRepository.GetItemAsync(message.Id.ToString());
            flashcard.IsDeleted = true;

            flashcardMeta.IsDeleted = true; // for update flash cards count correctly
            flashcardMeta.Box.UpdateFlashcardCount();
            m_BoxRepository.Save(flashcardMeta.Box);
            var t1 =  m_FlashcardRepository.UpdateItemAsync(message.Id.ToString(), flashcard);
            m_FlashcardMetaRepository.Delete(flashcardMeta);
            var t2 =  m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(flashcardMeta.User.Id));
            await Task.WhenAll(t1, t2);
        }
    }
}