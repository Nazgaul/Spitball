using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class ChatRoomAdminMap : ClassMap<ChatRoomAdmin>
    {
        public ChatRoomAdminMap()
        {
            Id(x => x.Id).GeneratedBy.Foreign("ChatRoom");
            Map(x => x.Status).Column("Status2").CustomType< EnumerationType<ChatRoomStatus>>();
            Map(x => x.AssignTo).Length(255);

            HasOne(x => x.ChatRoom).Constrained().Cascade.None();
            References(x => x.Lead).Nullable();
        }
    }
}
