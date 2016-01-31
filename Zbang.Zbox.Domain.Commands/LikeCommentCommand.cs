using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class LikeCommentCommand : ICommand
    {
        public LikeCommentCommand(Guid commentId, long userId)
        {
            UserId = userId;
            CommentId = commentId;
        }

        public long UserId { get; private set; }
        public Guid CommentId { get; private set; }
    }
}
