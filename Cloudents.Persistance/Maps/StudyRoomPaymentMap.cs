using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class StudyRoomPaymentMap : ClassMap<StudyRoomPayment>
    {
        public StudyRoomPaymentMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
           
            References(x => x.StudyRoomSessionUser).Cascade.None().LazyLoad();
            Map(x => x.PricePerHour);
            Map(x => x.TutorApproveTime);
            Map(x => x.TotalPrice);
            Map(x => x.Receipt).Nullable();
            Map(x => x.Created).Column("Create");

            References(x => x.User).Not.Nullable();
            References(x => x.Tutor).Not.Nullable();
            References(x => x.StudyRoom).Nullable();
        }
    }
}