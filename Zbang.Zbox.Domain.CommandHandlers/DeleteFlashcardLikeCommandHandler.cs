using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteFlashcardLikeCommandHandler : ICommandHandlerAsync<DeleteFlashcardLikeCommand>
    {
        private readonly IRepository<FlashcardLike> m_FlashcardLikeRepository;
        private readonly IRepository<FlashcardMeta> m_FlashcardRepository;
        private readonly IQueueProvider m_QueueProvider;

        public DeleteFlashcardLikeCommandHandler(IRepository<FlashcardLike> flashcardLikeRepository, IRepository<FlashcardMeta> flashcardRepository, IQueueProvider queueProvider)
        {
            m_FlashcardLikeRepository = flashcardLikeRepository;
            m_FlashcardRepository = flashcardRepository;
            m_QueueProvider = queueProvider;
        }

        public Task HandleAsync(DeleteFlashcardLikeCommand message)
        {
            var like = m_FlashcardLikeRepository.Load(message.Id);
            if (like.User.Id != message.UserId)
            {
                throw new UnauthorizedAccessException();
            }
            like.Flashcard.LikeCount--;
            like.Flashcard.ShouldMakeDirty = () => true;
            m_FlashcardLikeRepository.Delete(like);
            m_FlashcardRepository.Save(like.Flashcard);
            var t1 = m_QueueProvider.InsertMessageToTransactionAsync(new ReputationData(like.Flashcard.User.Id));
            var t2 = m_QueueProvider.InsertMessageToTransactionAsync(new LikesBadgeData(message.UserId));
            return Task.WhenAll(t1, t2);

        }
    }
}