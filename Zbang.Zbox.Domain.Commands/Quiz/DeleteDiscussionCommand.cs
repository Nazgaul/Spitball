using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class DeleteDiscussionCommand : ICommand
    {
        public DeleteDiscussionCommand(Guid discussionId, long userId)
        {
            DiscussionId = discussionId;
            UserId = userId;
        }

        public Guid DiscussionId { get; private set; }
        public long UserId { get; private set; }
    }
}
