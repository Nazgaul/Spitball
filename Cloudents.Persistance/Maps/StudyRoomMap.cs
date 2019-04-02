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
            HasMany(x => x.Users).Cascade.AllDeleteOrphan();
            Map(x => x.DateTime).Not.Nullable();
            Map(x => x.OnlineDocumentUrl).Not.Nullable();
        }

    }

    public class StudyRoomUserMap : ClassMap<StudyRoomUser>
    {
        public StudyRoomUserMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.User).Not.Nullable();

        }
    }
}