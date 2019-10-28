using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public class StudyRoomMap : ClassMap<StudyRoom>
    {
        public StudyRoomMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Identifier).Not.Nullable().Unique();
            References(x => x.Tutor).Not.Nullable();

            Map(x => x.Type);
            Map(x => x.DateTime).Not.Nullable();
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