using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddNodeToLibraryCommand : ICommand, ICommandCache
    {
        public AddNodeToLibraryCommand(string name, long universityId , Guid? parentId, long userId)
        {
            UserId = userId;

            Name = name ?? throw new ArgumentNullException(nameof(name));
            UniversityId = universityId;
            ParentId = parentId;
        }
        public string Name { get; private set; }
        public long UniversityId { get; }
        public Guid? ParentId { get; private set; }
        public long UserId { get; private set; }


        public Guid Id { get; set; }
        public CacheRegions CacheRegion => CacheRegions.BuildNodesRegion(UniversityId);
    }
}
