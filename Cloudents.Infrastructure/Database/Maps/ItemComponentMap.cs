using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;

namespace Cloudents.Infrastructure.Database.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    public class ItemComponentMap : ComponentMap<ItemComponent>
    {
        public ItemComponentMap()
        {
            Map(m => m.State).CustomType<GenericEnumStringType<ItemState>>().Not.Nullable();
            Map(m => m.DeletedOn).Nullable();
            Map(m => m.FlagReason).Nullable().Length(255);

            Map(m => m.VoteCount).Not.Nullable();
            HasMany(x => x.Votes).Inverse().Cascade.AllDeleteOrphan();
        }
    }
}