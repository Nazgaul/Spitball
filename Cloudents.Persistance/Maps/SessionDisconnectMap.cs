using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class StudyRoomSessionUserMap : ClassMap<StudyRoomSessionUser>
    {
        public StudyRoomSessionUserMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(r => r.StudyRoomSession).Column("SessionId")
                .Not.Nullable();
            References(x => x.User).Not.Nullable();

            Map(x => x.Duration).Nullable();
        }
    }
}
