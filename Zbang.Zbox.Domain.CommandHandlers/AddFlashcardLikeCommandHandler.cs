using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddFlashcardLikeCommandHandler : ICommandHandlerAsync<AddFlashcardLikeCommand>
    {
        private readonly IRepository<FlashcardLike> m_FlashcardLikeRepository;
        private readonly IRepository<FlashcardMeta> m_FlashcardRepository;
        private readonly IGuidIdGenerator m_GuidGenerator;
        private readonly IUserRepository m_UserRepository;
        private readonly IQueueProvider m_QueueProvider;

        public AddFlashcardLikeCommandHandler( IRepository<FlashcardMeta> flashcardRepository, IGuidIdGenerator guidGenerator, IUserRepository userRepository, IRepository<FlashcardLike> flashcardLikeRepository, IQueueProvider queueProvider)
        {
            m_FlashcardRepository = flashcardRepository;
            m_GuidGenerator = guidGenerator;
            m_UserRepository = userRepository;
            m_FlashcardLikeRepository = flashcardLikeRepository;
            m_QueueProvider = queueProvider;
        }

        public Task HandleAsync(AddFlashcardLikeCommand message)
        {
            var user = m_UserRepository.Load(message.UserId);
            var flashcard = m_FlashcardRepository.Load(message.FlashcardId);
            flashcard.LikeCount++;
            flashcard.ShouldMakeDirty = () => true;
            var like = new FlashcardLike(m_GuidGenerator.GetId(), user, flashcard);
            m_FlashcardLikeRepository.Save(like);
            m_FlashcardRepository.Save(flashcard);
            message.Id = like.Id;
            var t1 = m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(like.Flashcard.User.Id));
            var t2 = m_QueueProvider.InsertMessageToTranactionAsync(new LikesBadgeData(message.UserId));
            return Task.WhenAll(t1, t2);
        }
    }
}
