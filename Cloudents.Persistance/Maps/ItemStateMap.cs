using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    public class ItemStateMap : ComponentMap<ItemStatus>
    {
        public ItemStateMap()
        {
            Map(x => x.State)
                .CustomType<GenericEnumStringType<ItemState>>().Not.Nullable();
            Map(m => m.DeletedOn).Nullable();
            Map(m => m.FlagReason).Nullable();
            References(x => x.FlaggedUser).Column("FlaggedUserId");
        }
    }
}