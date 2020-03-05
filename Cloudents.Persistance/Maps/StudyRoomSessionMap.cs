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
            Map(x => x.DurationInMinutes).CustomType<MinuteTimeSpanType>();
            Map(x => x.RealDuration);
            Map(x => x.SessionId).Not.Nullable();
            Map(x => x.Receipt);
            Map(x => x.Price).CustomSqlType("smallMoney").Nullable();
            Map(x => x.VideoExists);
            HasMany(x => x.ParticipantDisconnections).Access.CamelCaseField(Prefix.Underscore)
               .KeyColumn("SessionId")
               .Inverse().Cascade.AllDeleteOrphan();
            Map(x => x.PaymentApproved).Nullable();
            Map(x => x.AdminDuration).Nullable();
            Map(x => x.StudentPay).CustomSqlType("smallMoney").Nullable();
            Map(x => x.SpitballPay).CustomSqlType("smallMoney").Nullable();
            Version(x => x.Version).CustomSqlType("timestamp").Generated.Always();
        }
    }
}
