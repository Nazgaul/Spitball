using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Data.Maps
{
    [UsedImplicitly]
    public class UniversityMap : SpitballClassMap<University>
    {
        public UniversityMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.IsDeleted);
            //TODO : this appear twice - need to fix that
            Map(x => x.Name).Column("UniversityName");
            Map(x => x.Extra);
            Map(x => x.ExtraSearch);
            Schema("Zbox");
        }
    }
}