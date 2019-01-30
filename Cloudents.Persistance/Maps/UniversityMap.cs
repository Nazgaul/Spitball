using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistance.Maps
{
    public sealed class UniversityMap : ClassMap<University>
    {
        public UniversityMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            //TODO: need to merge field in university before create constraint
            Map(x => x.Name); 
            Map(x => x.Extra);
            //Map(x => x.Country).Not.Nullable().Length(2).UniqueKey("uq_UniversityNameCountry");
            Component(x => x.RowDetail);

            SchemaAction.Update();

        }
    }
}