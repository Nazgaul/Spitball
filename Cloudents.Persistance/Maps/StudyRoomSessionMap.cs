using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using System;

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
            Map(m => m.PaymentApproved).Nullable();
            Map(x => x.AdminDuration).Nullable();
            //ReferencesAny(x => x.Payment).Cascade.All()
            //.AddMetaValue<Payme>("Payme")
            //.AddMetaValue<PayPal>("PayPal")
            //.EntityTypeColumn("Type")
            //.EntityIdentifierColumn("PaymentId")
            //.IdentityType<Guid>()
            //.MetaType<string>();

            Version(x => x.Version).CustomSqlType("timestamp").Generated.Always();
        }
    }
}
