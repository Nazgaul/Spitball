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

            HasOne(x => x.StudyRoomPayment)
                .Constrained().PropertyRef(x=>x!.StudyRoomSessionUser)
                .Cascade.All().LazyLoad(Laziness.NoProxy);

            HasMany(x => x.UserCoupons).KeyColumn("SessionUserId").Inverse().Cascade.AllDeleteOrphan();

            Map(x => x.Duration).Nullable();
            Map(x => x.DisconnectCount);

        }
    }
}
