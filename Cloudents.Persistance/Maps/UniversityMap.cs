using Cloudents.Domain.Entities;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Database.Maps
{
    public sealed class UniversityMap : ClassMap<University>
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