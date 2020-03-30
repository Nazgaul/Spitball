using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class StudyRoomSessionMap : ClassMap<StudyRoomSession>
    {
        public StudyRoomSessionMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.StudyRoom).Column("StudyRoomId").ForeignKey("Session_Room").Not.Nullable();
            Map(x => x.Created).Not.Nullable();
            Map(x => x.Ended);
            Map(x => x.RejoinCount);
            Map(x => x.Duration);
            Map(x => x.RealDuration);
            Map(x => x.SessionId).Not.Nullable();
            Map(x => x.Receipt);
            Map(x => x.Price).CustomSqlType("smallMoney").Nullable();
            //Map(x => x.VideoExists);
            HasMany(x => x.RoomSessionUsers)
               .KeyColumn("SessionId")
               .Inverse().Cascade.AllDeleteOrphan();
            Map(m => m.PaymentApproved).Nullable();
            Map(x => x.AdminDuration).Column("AdminDuration2").Nullable();

            Version(x => x.Version).CustomSqlType("timestamp").Generated.Always();
        }
    }
}
