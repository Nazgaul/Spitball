using System.Diagnostics.CodeAnalysis;
using Cloudents.Domain.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Infrastructure.Database.Maps
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