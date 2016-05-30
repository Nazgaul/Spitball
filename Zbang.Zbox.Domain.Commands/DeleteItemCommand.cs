using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteItemCommand : ICommandAsync, ICommandCache
    {

        public DeleteItemCommand(long itemId, long userId)
        {
            ItemId = itemId;
            UserId = userId;
        }

        public long ItemId
        {
            get;
            private set;

        }

        public long BoxId { get;  set; }

        public long UserId { get; private set; }


        public string CacheRegion => CacheRegions.BuildFeedRegion(BoxId);
    }
}
