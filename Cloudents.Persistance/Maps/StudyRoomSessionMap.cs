using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using JetBrains.Annotations;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public class StudyRoomSessionMap : ClassMapping<StudyRoomSession>
    {
        public StudyRoomSessionMap()
        {
            Id(x => x.Id, c => c.Generator(Generators.GuidComb));
            //Id(x => x.Id).GeneratedBy.GuidComb();
            ManyToOne(x => x.StudyRoom, c => {
                c.Column("StudyRoomId");
                c.ForeignKey("Session_Room");
                c.NotNullable(true);
            });
            //References(x => x.StudyRoom).Column("StudyRoomId").ForeignKey("Session_Room").Not.Nullable();
            Property(x => x.Created, c => c.NotNullable(true));
            //Map(x => x.Created).Not.Nullable();
            Property(x => x.Ended);
            //Map(x => x.Ended);
            Property(x => x.RejoinCount);
            //Map(x => x.RejoinCount);
            Property(x => x.Duration);
            //Map(x => x.Duration);
            Property(x => x.DurationInMinutes, c => c.Type<MinuteTimeSpanType>());
            //Map(x => x.DurationInMinutes).CustomType<MinuteTimeSpanType>();
            Property(x => x.SessionId, c => c.NotNullable(true));
            //Map(x => x.SessionId).Not.Nullable();
            Property(x => x.Receipt);
            //Map(x => x.Receipt);

            DynamicUpdate(true);
            OptimisticLock(OptimisticLockMode.Version);
            Version(x => x.Version, c => {
                c.Generated(VersionGeneration.Always);
                c.Column(cl => {
                    cl.SqlType("timestamp");
                });
            });
            //Version(x => x.Version).CustomSqlType("rowversion").Generated.Always();
        }
    }
}
