using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class LikeCommentCommand : ICommand
    {
        public LikeCommentCommand(Guid commentId, long userId, long boxId)
        {
            BoxId = boxId;
            UserId = userId;
            CommentId = commentId;
        }

        public long UserId { get; private set; }
        public Guid CommentId { get; private set; }
        public long BoxId { get; private set; }
    }

    public class LikeCommentCommandResult : ICommandResult
    {
        public LikeCommentCommandResult(bool liked)
        {
            Liked = liked;
        }

        public bool Liked { get; private set; }
    }
}
