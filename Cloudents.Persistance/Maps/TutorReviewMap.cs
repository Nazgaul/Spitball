using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class TutorReviewMap : ClassMap<TutorReview>
    {
        public TutorReviewMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.User).Not.Nullable().Column("UserId");
            References(x => x.Tutor).Not.Nullable().Column("TutorId");

            Map(x => x.DateTime).Not.Nullable();
            Map(x => x.Review).Not.Nullable().Length(1000);
            Map(x => x.Rate).Not.Nullable();
            SchemaAction.Update();
            Table("TutorReview");
        }
    }
}