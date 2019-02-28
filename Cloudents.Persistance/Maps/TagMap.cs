using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistance.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    public sealed class TagMap : ClassMap<Tag>
    {
        public TagMap()
        {
            Table("Tag2");
            Id(e => e.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10",
                $"{nameof(HiLoGenerator.TableName)}='{nameof(Tag)}'");
            Map(e => e.Name).Length(150);
            Map(x => x.Count).Not.Nullable();

            SchemaAction.None();
        }
    }
}