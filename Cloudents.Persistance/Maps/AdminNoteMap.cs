using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class AdminNoteMap : ClassMap<AdminNote>
    {
        public AdminNoteMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Text).Length(4000);
            Component(x => x.TimeStamp);
            References(x => x.User).Column("UserId").Not.Nullable().ForeignKey("FK_note_user");
            References(x => x.AdminUser).Column("AdminUserId").Not.Nullable().ForeignKey("FK_note_adminUser");
        }
    }
}
