using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    public sealed class TagMap : ClassMapping<Tag>
    {
        public TagMap()
        {
            Id(e => e.Name, c => {
                c.Generator(Generators.Assigned);
                c.Length(150);
            });
            //Id(e => e.Name).GeneratedBy.Assigned().Length(150);
            Property(x => x.Count, c => c.NotNullable(true));
            //Map(x => x.Count).Not.Nullable();
            SchemaAction(NHibernate.Mapping.ByCode.SchemaAction.Validate);
            //SchemaAction.Validate();
        }
    }
}