using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
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

            HasMany(x => x.Documents)
                .ReadOnly()
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.None();
            //HasMany(x => x.Questions)
            //    .ReadOnly()
            //    .Access.CamelCaseField(Prefix.Underscore).Cascade.None();
            //HasMany(x => x.Users)
            //    .ReadOnly()
            //    .Access.CamelCaseField(Prefix.Underscore).Cascade.None();
            Map(x => x.State).Not.Nullable();
            SchemaAction.None();

        }
    }
}