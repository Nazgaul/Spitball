using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
//using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public sealed class UniversityMap : ClassMapping<University>
    {
        public UniversityMap()
        {
            DynamicUpdate(true);
            //DynamicUpdate();
            Id(x => x.Id, c => c.Generator(Generators.GuidComb));
            //Id(x => x.Id).GeneratedBy.GuidComb();
            Property(x => x.Name, c => c.UniqueKey("uq_UniversityNameCountry"));
            //Map(x => x.Name).UniqueKey("uq_UniversityNameCountry");
            Property(x => x.Extra);
            //Map(x => x.Extra);
            Property(x => x.UsersCount);
            //Map(x => x.UsersCount);
            Property(x => x.Image, c => c.Column("ImageUrl"));
            //Map(x => x.Image).Column("ImageUrl");
            Property(x => x.Country, c =>
            {
                c.NotNullable(true);
                c.Length(2);
                c.UniqueKey("uq_UniversityNameCountry");
            });
            //Map(x => x.Country).Not.Nullable().Length(2).UniqueKey("uq_UniversityNameCountry");
            Component(x => x.RowDetail);
            //Component(x => x.RowDetail);

            ////HasMany(x => x.Documents)
            ////    .ReadOnly()
            ////    .Access.CamelCaseField(Prefix.Underscore)
            ////    .Cascade.None();
            ////HasMany(x => x.Questions)
            ////    .ReadOnly()
            ////    .Access.CamelCaseField(Prefix.Underscore).Cascade.None();
            ////HasMany(x => x.Users)
            ////    .ReadOnly()
            ////    .Access.CamelCaseField(Prefix.Underscore).Cascade.None();
            Property(x => x.State, c =>
            {
                c.Type<GenericEnumStringType<ItemState>>();
                c.NotNullable(true);
            });
            //Map(x => x.State).CustomType<GenericEnumStringType<ItemState>>().Not.Nullable();
            SchemaAction(NHibernate.Mapping.ByCode.SchemaAction.Validate);
            //SchemaAction.Validate();

        }
    }
}