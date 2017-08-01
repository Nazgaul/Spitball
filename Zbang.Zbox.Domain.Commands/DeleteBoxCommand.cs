using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteBoxCommand : ICommandAsync, ICommandCache
    {
        public DeleteBoxCommand(long boxId)
        {
            BoxId = boxId;
        }

        public long UniversityId { get; set; }

        public long BoxId { get;private set; }
        public CacheRegions CacheRegion => CacheRegions.BuildNodesRegion(UniversityId);
    }
}
