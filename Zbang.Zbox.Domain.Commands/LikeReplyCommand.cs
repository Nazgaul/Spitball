using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class LikeReplyCommand : ICommandAsync, ICommandCache
    {
        public LikeReplyCommand(Guid replyId, long userId, long boxId)
        {
            BoxId = boxId;
            UserId = userId;
            ReplyId = replyId;
        }

        public long UserId { get; private set; }
        public Guid ReplyId { get; private set; }

        public long BoxId { get; }
        public string CacheRegion => CacheRegions.BuildFeedRegion(BoxId);
    }
}
