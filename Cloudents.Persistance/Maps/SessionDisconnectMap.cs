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
                .Not.Nullable().UniqueKey("k_StudyRoomSessionUser");
            References(x => x.User).Not.Nullable().UniqueKey("k_StudyRoomSessionUser");

            Map(x => x.Duration).Nullable();
            Map(x => x.DisconnectCount);
            Map(x => x.PricePerHour);
            Map(x => x.TutorApproveTime);
            Map(x => x.TotalPrice);
            Map(x => x.Receipt).Nullable();

        }
    }
}
