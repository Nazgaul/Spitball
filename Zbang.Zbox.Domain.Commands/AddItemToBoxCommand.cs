using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public abstract class AddItemToBoxCommand : ICommandAsync, ICommandCache
    {
        protected AddItemToBoxCommand(long userId, long boxId)
        {
            BoxId = boxId;
            UserId = userId;
        }

        public const string FileResolver = "File";
        public const string LinkResolver = "Link";
        public abstract string ResolverName { get; }


        public long UserId { get; private set; }

        public long BoxId { get; }
        public CacheRegions CacheRegion => CacheRegions.BuildFeedRegion(BoxId);
    }
}
