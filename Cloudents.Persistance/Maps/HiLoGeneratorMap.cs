using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using System.Diagnostics.CodeAnalysis;
//using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    public class HiLoGeneratorMap : ClassMapping<HiLoGenerator>
    {
        public HiLoGeneratorMap()
        {
            Id(x => x.Id, c => c.Generator(Generators.Native));
            //Id(x => x.Id).GeneratedBy.Native();
            Property(x => x.TableName, c => c.NotNullable(true));
            //Map(x => x.TableName).Not.Nullable();
            Property(x => x.NextHi, c => c.NotNullable(true));
            //Map(x => x.NextHi).Not.Nullable();
            SchemaAction(NHibernate.Mapping.ByCode.SchemaAction.None);
            //SchemaAction.None();
        }
    }
}