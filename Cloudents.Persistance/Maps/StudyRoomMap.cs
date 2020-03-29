using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class StudyRoomMap : ClassMap<StudyRoom>
    {
        public StudyRoomMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Identifier).Not.Nullable().Unique();
            References(x => x.Tutor).Not.Nullable();

            Map(x => x.Type);
            Map(x => x.Name).Length(500);
            Component(x => x.DateTime, z => {
                z.Map(m => m.CreationTime).Column("DateTime");
                z.Map(m => m.UpdateTime).Column("Updated");
            });
            Map(x => x.OnlineDocumentUrl).Not.Nullable();
            HasMany(x => x.Sessions).Access.CamelCaseField(Prefix.Underscore)
                .KeyColumn("StudyRoomId")
                .Inverse().Cascade.AllDeleteOrphan();


            HasMany(x => x.Users).Access.CamelCaseField(Prefix.Underscore)
                .Inverse().Cascade.AllDeleteOrphan();


        }

    }

    public class StudyRoomUserMap : ClassMap<StudyRoomUser>
    {
        public StudyRoomUserMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.User).Not.Nullable();
            References(x => x.Room).Column("StudyRoomId").Not.Nullable();
            Map(x => x.Online).Not.Nullable();
        }
    }
}