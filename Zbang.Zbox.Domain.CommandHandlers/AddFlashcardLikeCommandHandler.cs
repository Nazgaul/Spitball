using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddFlashcardLikeCommandHandler : ICommandHandler<AddFlashcardLikeCommand>
    {
        private readonly IRepository<FlashcardLike> m_FlashcardLikeRepository;
        private readonly IRepository<FlashcardMeta> m_FlashcardRepository;
        private readonly IGuidIdGenerator m_GuidGenerator;
        private readonly IUserRepository m_UserRepository;

        public AddFlashcardLikeCommandHandler( IRepository<FlashcardMeta> flashcardRepository, IGuidIdGenerator guidGenerator, IUserRepository userRepository, IRepository<FlashcardLike> flashcardLikeRepository)
        {
            m_FlashcardRepository = flashcardRepository;
            m_GuidGenerator = guidGenerator;
            m_UserRepository = userRepository;
            m_FlashcardLikeRepository = flashcardLikeRepository;
        }

        public void Handle(AddFlashcardLikeCommand message)
        {

            var user = m_UserRepository.Load(message.UserId);
            var flashcard = m_FlashcardRepository.Load(message.FlashcardId);
            flashcard.LikeCount++;
            var like = new FlashcardLike(m_GuidGenerator.GetId(), user, flashcard);
            m_FlashcardLikeRepository.Save(like);
            m_FlashcardRepository.Save(flashcard);
            message.Id = like.Id;
        }
    }
}
