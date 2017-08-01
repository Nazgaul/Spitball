using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class LikeCommentCommand : ICommandAsync, ICommandCache
    {
        public LikeCommentCommand(Guid commentId, long userId, long boxId)
        {
            BoxId = boxId;
            UserId = userId;
            CommentId = commentId;
        }

        public long UserId { get; private set; }
        public Guid CommentId { get; private set; }
        public long BoxId { get; }
        public CacheRegions CacheRegion => CacheRegions.BuildFeedRegion(BoxId);
    }
}
