using System;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    public class DeleteDiscussionCommandHandler : ICommandHandler<DeleteDiscussionCommand>
    {
        private readonly IRepository<Discussion> m_DiscussionRepsitory;

        public DeleteDiscussionCommandHandler(IRepository<Discussion> discussionRepository)
        {
            m_DiscussionRepsitory = discussionRepository;
        }

        public void Handle(DeleteDiscussionCommand message)
        {
            var disucssionItem = m_DiscussionRepsitory.Load(message.DiscussionId);

            if (disucssionItem.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("user is not owner of this item");
            }
            m_DiscussionRepsitory.Delete(disucssionItem);
        }
    }
}
