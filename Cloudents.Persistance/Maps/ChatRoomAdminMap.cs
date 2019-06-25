using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class ChatRoomAdminMap : ClassMap<ChatRoomAdmin>
    {
        public ChatRoomAdminMap()
        {
            Id(x => x.Id).GeneratedBy.Foreign("ChatRoom");
            Map(x => x.Status);
            Map(x => x.AssignTo).Length(20);

            HasOne(x => x.ChatRoom).Constrained().Cascade.None();
            References(x => x.Lead).Nullable();
        }
    }
}
