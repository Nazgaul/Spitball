using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteUpdatesCommand :ICommand
    {
        public DeleteUpdatesCommand(long userId, long boxId)
        {
            UserId = userId;
            BoxId = boxId;
        }
        public long UserId { get; private set; }
        public long BoxId { get;private set; }
    }

    public class DeleteUpdatesFeedCommand : DeleteUpdatesCommand
    {
        public DeleteUpdatesFeedCommand(long userId, long boxId,Guid commentId)
            :base(userId,boxId)
        {
            CommentId = commentId;
        }

        public Guid  CommentId { get; private set; }
    }

    public class DeleteUpdatesItemCommand : DeleteUpdatesCommand
    {
        public DeleteUpdatesItemCommand(long userId, long boxId)
            :base(userId,boxId)
        {
            
        }
    }
}
