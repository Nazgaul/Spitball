using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public class TutorReviewMap : ClassMap<TutorReview>
    {
        public TutorReviewMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.User).Not.Nullable().Column("UserId");
            References(x => x.Tutor).Not.Nullable().Column("TutorId");
            //References(x => x.Room).Not.Nullable().Column("RoomId");
            Map(x => x.DateTime).Not.Nullable();
            Map(x => x.Review).Length(1000);
            Map(x => x.Rate).Not.Nullable();
            Map(x => x.IsShownHomePage);

            DynamicUpdate();
            OptimisticLock.Version();
            Version(x => x.Version).CustomSqlType("timestamp").Generated.Always();

        }
    }
}