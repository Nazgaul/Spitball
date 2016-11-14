using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteFlashcardLikeCommandHandler : ICommandHandler<DeleteFlashcardLikeCommand>
    {
        private readonly IRepository<FlashcardLike> m_FlashcardLikeRepository;
        private readonly IRepository<FlashcardMeta> m_FlashcardRepository;

        public DeleteFlashcardLikeCommandHandler(IRepository<FlashcardLike> flashcardLikeRepository, IRepository<FlashcardMeta> flashcardRepository)
        {
            m_FlashcardLikeRepository = flashcardLikeRepository;
            m_FlashcardRepository = flashcardRepository;
        }

        public void Handle(DeleteFlashcardLikeCommand message)
        {
            var like = m_FlashcardLikeRepository.Load(message.Id);
            if (like.User.Id != message.UserId)
            {
                throw new UnauthorizedAccessException();
            }
            like.Flashcard.NumberOfViews--;

            m_FlashcardLikeRepository.Delete(like);
            m_FlashcardRepository.Save(like.Flashcard);

        }
    }
}