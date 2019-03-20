using System.Diagnostics.CodeAnalysis;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Fluent nhibernate")]
    public class HiLoGeneratorMap : ClassMap<HiLoGenerator>
    {
        public HiLoGeneratorMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.TableName).Not.Nullable();
            Map(x => x.NextHi).Not.Nullable();

            SchemaAction.None();
        }
    }
}