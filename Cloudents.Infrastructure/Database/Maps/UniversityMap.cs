using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Database.Maps
{
    [UsedImplicitly]
    public sealed class UniversityMap : SpitballClassMap<University>
    {
        public UniversityMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Name).UniqueKey("uq_UniversityNameCountry");
            Map(x => x.Extra);
            Map(x => x.Country).Not.Nullable().Length(2).UniqueKey("uq_UniversityNameCountry");
            Component(x => x.RowDetail);

            SchemaAction.None();

        }
    }
}