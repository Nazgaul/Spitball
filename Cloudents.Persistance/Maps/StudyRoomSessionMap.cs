using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
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
            Map(x => x.SessionId).Not.Nullable();
            Map(x => x.Receipt);
            Map(x => x.Price).CustomSqlType("smallMoney").Nullable();
            Map(x => x.VideoExists);
            HasMany(x => x.ParticipantDisconnections).Access.CamelCaseField(Prefix.Underscore)
               .KeyColumn("SessionId")
               .Inverse().Cascade.AllDeleteOrphan();
            Version(x => x.Version).CustomSqlType("timestamp").Generated.Always();
        }
    }
}
