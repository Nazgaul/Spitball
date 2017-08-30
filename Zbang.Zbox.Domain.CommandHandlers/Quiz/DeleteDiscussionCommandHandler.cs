using System;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    public class DeleteDiscussionCommandHandler : ICommandHandler<DeleteDiscussionCommand>
    {
        private readonly IRepository<QuizDiscussion> m_DiscussionRepository;
        private readonly IUpdatesRepository m_UpdatesRepository;

        public DeleteDiscussionCommandHandler(IRepository<QuizDiscussion> discussionRepository, IUpdatesRepository updatesRepository
            )
        {
            m_DiscussionRepository = discussionRepository;
            m_UpdatesRepository = updatesRepository;
        }

        public void Handle(DeleteDiscussionCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var discussionItem = m_DiscussionRepository.Load(message.DiscussionId);

            if (discussionItem.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("user is not owner of this item");
            }
            m_UpdatesRepository.DeleteQuizDiscussionUpdates(message.DiscussionId, discussionItem.Quiz.Id);
            m_DiscussionRepository.Delete(discussionItem);
        }
    }
}
