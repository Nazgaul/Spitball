using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Database.Maps
{
    [UsedImplicitly]
    public class UniversityMap : SpitballClassMap<University>
    {
        public UniversityMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.IsDeleted);
            Map(x => x.Name).Column("UniversityName");
            Map(x => x.Extra);
            Map(x => x.ExtraSearch);
            Map(x => x.Latitude).Nullable();
            Map(x => x.Longitude).Nullable();
            Map(x => x.Image).Column("LargeImage").Nullable();
            Map(x => x.Country).Not.Nullable().Length(2);
            Schema("Zbox");
        }
    }
}