using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
//using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class ChatRoomAdminMap : ClassMapping<ChatRoomAdmin>
    {
        public ChatRoomAdminMap()
        {
            Id(x => x.Id, c => {
                c.Generator(Generators.Foreign<ChatRoom>(x => x.Id));
            });
            //Id(x => x.Id).GeneratedBy.Foreign("ChatRoom");
            Property(x => x.Status, c => {
                c.Column("Status2");
                c.Type<EnumerationType<ChatRoomStatus>>();
            });
            //Map(x => x.Status).Column("Status2").CustomType<EnumerationType<ChatRoomStatus>>();
            Property(x => x.AssignTo, c => { });
            //Map(x => x.AssignTo).Length(20);
            OneToOne(x => x.ChatRoom, c => {
                c.Cascade(Cascade.None);
                c.Constrained(true);
            });
            //HasOne(x => x.ChatRoom).Constrained().Cascade.None();
            
            ManyToOne(x => x.Lead, c => {
                c.NotNullable(false);
                c.Column("LeadId");
            });
            //References(x => x.Lead).Nullable();
        }
    }
}
