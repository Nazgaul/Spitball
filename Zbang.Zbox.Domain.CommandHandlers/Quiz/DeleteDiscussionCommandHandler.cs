using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    public class DeleteDiscussionCommandHandler : ICommandHandler<DeleteDiscussionCommand>
    {
        private readonly IRepository<Discussion> m_DiscussionRepository;
        private readonly IRepository<Domain.Quiz> m_QuizRepository;

        public DeleteDiscussionCommandHandler(IRepository<Discussion> discussionRepository,
             IRepository<Domain.Quiz> quizRepository)
        {
            m_DiscussionRepository = discussionRepository;
            m_QuizRepository = quizRepository;
        }

        public void Handle(DeleteDiscussionCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var discussionItem = m_DiscussionRepository.Load(message.DiscussionId);

            if (discussionItem.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("user is not owner of this item");
            }
            var noOfDiscussion = m_DiscussionRepository.GetQuerable().Count(w => w.Quiz == discussionItem.Quiz);
            discussionItem.Quiz.UpdateNumberOfComments(noOfDiscussion - 1); // the current one is not saved yet
            m_QuizRepository.Save(discussionItem.Quiz);
            m_DiscussionRepository.Delete(discussionItem);
        }
    }
}
