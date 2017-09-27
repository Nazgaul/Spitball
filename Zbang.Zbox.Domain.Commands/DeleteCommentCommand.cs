using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteCommentCommand : ICommandAsync, ICommandCache
    {
        public DeleteCommentCommand(Guid commentId, long userId, long boxId)
        {
            CommentId = commentId;
            UserId = userId;
            BoxId = boxId;
        }

        public Guid CommentId { get; private set; }

        public long UserId { get; private set; }

        public long BoxId { get; }
        public CacheRegions CacheRegion => CacheRegions.BuildFeedRegion(BoxId);
    }
}
