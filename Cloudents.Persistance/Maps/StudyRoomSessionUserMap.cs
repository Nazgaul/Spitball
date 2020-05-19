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

            HasOne(x => x.StudyRoomPayment).Constrained()
                .Cascade.All().LazyLoad(Laziness.NoProxy);

            Map(x => x.Duration).Nullable();
            Map(x => x.DisconnectCount);
            Map(x => x.PricePerHour);
            Map(x => x.TutorApproveTime);
            Map(x => x.TotalPrice);
            Map(x => x.Receipt).Nullable();

        }
    }

    public class StudyRoomPaymentMap : ClassMap<StudyRoomPayment>
    {
        public StudyRoomPaymentMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
           

            //HasOne(x => x.StudyRoomSessionUser).Cascade.None()/*.LazyLoad(Laziness.NoProxy)*/;
            References(x => x.StudyRoomSessionUser).Unique().Cascade.None().LazyLoad();

            Map(x => x.PricePerHour);
            Map(x => x.TutorApproveTime);
            Map(x => x.TotalPrice);
            Map(x => x.Receipt).Nullable();
            Map(x => x.Create);

            References(x => x.User).Not.Nullable();
            References(x => x.Tutor).Not.Nullable();
        }
    }

}
