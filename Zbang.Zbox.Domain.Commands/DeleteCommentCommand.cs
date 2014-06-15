using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteCommentCommand : ICommand
    {
        public DeleteCommentCommand(Guid questionId, long userId)
        {
            QuestionId = questionId;
            UserId = userId;
        }
        public Guid QuestionId { get; private set; }

        public long UserId { get; private set; }
    }
}
