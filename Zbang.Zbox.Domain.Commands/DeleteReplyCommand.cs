using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteReplyCommand : ICommandAsync, ICommandCache
    {
        public DeleteReplyCommand(Guid answerId, long userId, long boxId)
        {
            AnswerId = answerId;
            UserId = userId;
            BoxId = boxId;
        }

        public Guid AnswerId { get; set; }

        public long UserId { get; set; }
        public long BoxId { get; }
        public CacheRegions CacheRegion => CacheRegions.BuildFeedRegion(BoxId);
    }
}
