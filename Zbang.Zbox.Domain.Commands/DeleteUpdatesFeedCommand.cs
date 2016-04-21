using System;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteUpdatesFeedCommand : DeleteUpdatesCommand
    {
        public DeleteUpdatesFeedCommand(long userId, long boxId,Guid commentId)
            :base(userId,boxId)
        {
            CommentId = commentId;
        }

        public Guid  CommentId { get; private set; }
    }
}