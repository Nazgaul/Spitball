using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteCommentCommand : ICommandAsync
    {
        public DeleteCommentCommand(Guid commentId, long userId)
        {
            CommentId = commentId;
            UserId = userId;
        }
        public Guid CommentId { get; private set; }

        public long UserId { get; private set; }
    }
}
