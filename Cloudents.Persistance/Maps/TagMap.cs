using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    public sealed class TagMap : ClassMap<Tag>
    {
        public TagMap()
        {
            Id(e => e.Name).GeneratedBy.Assigned().Length(150);
            Map(x => x.Count).Not.Nullable();

            SchemaAction.None();
        }
    }
}